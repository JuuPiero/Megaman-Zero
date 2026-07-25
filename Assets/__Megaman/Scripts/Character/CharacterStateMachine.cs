using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Megaman
{
    public enum StateLayer
    {
        Base = 0,      // Di chuyển, idle, jump (full body)
        UpperBody = 1, // Attack, shoot (upper body only)
        FullBody = 2   // Override toàn bộ (ví dụ: hurt, death)
    }
    public class CharacterStateMachine : MonoBehaviour
    {
        [Header("States Configuration")]
        public List<StateConfigSO> allStates = new List<StateConfigSO>();

        [Header("Layer Settings")]
        public AnimancerComponent animancer;
        public AvatarMask upperBodyMask; // Mask cho upper body

        // Layer management
        private Dictionary<StateLayer, AnimancerLayer> animancerLayers = new Dictionary<StateLayer, AnimancerLayer>();
        private Dictionary<StateLayer, StateConfigSO> currentLayerStates = new Dictionary<StateLayer, StateConfigSO>();
        private Dictionary<StateLayer, List<StateConfigSO>> statesByLayer = new Dictionary<StateLayer, List<StateConfigSO>>();

        // Properties
        public StateConfigSO BaseState => GetLayerState(StateLayer.Base);
        public StateConfigSO UpperBodyState => GetLayerState(StateLayer.UpperBody);
        public StateConfigSO FullBodyState => GetLayerState(StateLayer.FullBody);

        // Events
        public System.Action<StateLayer, StateConfigSO> OnStateChanged;

        private void Awake()
        {
            if (animancer == null)
                animancer = GetComponentInChildren<AnimancerComponent>();

            SetupLayers();
            OrganizeStatesByLayer();
        }

        

        private void SetupLayers()
        {
            // Base layer (luôn có sẵn)
            animancerLayers[StateLayer.Base] = animancer.Layers[0];
            animancerLayers[StateLayer.Base].SetWeight(1f);

            // Upper body layer
            if (animancer.Layers.Count > 1)
            {
                animancerLayers[StateLayer.UpperBody] = animancer.Layers[1];
                if (upperBodyMask != null)
                    animancerLayers[StateLayer.UpperBody].SetMask(upperBodyMask);
                animancerLayers[StateLayer.UpperBody].SetWeight(1f);
            }

            // Full body override layer
            if (animancer.Layers.Count > 2)
            {
                animancerLayers[StateLayer.FullBody] = animancer.Layers[2];
                animancerLayers[StateLayer.FullBody].SetWeight(0f); // Mặc định tắt
            }
        }

        private void OrganizeStatesByLayer()
        {
            foreach (StateLayer layer in System.Enum.GetValues(typeof(StateLayer)))
            {
                statesByLayer[layer] = new List<StateConfigSO>();
            }

            foreach (var state in allStates)
            {
                statesByLayer[state.stateLayer].Add(state);
            }
        }

        public void Initialize(BaseCharacter owner, InputManager input)
        {
            // Initialize tất cả states
            foreach (var state in allStates)
            {
                state.Initialize(owner, animancer, input);
            }

            // Bắt đầu với state đầu tiên của mỗi layer
            foreach (var layerStates in statesByLayer)
            {
                if (layerStates.Value.Count > 0)
                {
                    SetLayerState(layerStates.Key, layerStates.Value[0]);
                }
            }
        }

        /// <summary>
        /// Set state cho một layer cụ thể
        /// </summary>
        public void SetLayerState(StateLayer layer, StateConfigSO newState)
        {
            if (!statesByLayer[layer].Contains(newState))
            {
                Debug.LogError($"State {newState.stateName} không thuộc layer {layer}");
                return;
            }

            // Lấy state cũ của layer này
            StateConfigSO previousState = null;
            if (currentLayerStates.ContainsKey(layer))
                previousState = currentLayerStates[layer];

            // Kiểm tra conflict với FullBody state
            if (HasFullBodyState() && layer != StateLayer.FullBody)
            {
                // FullBody state đang active, không cho phép thay đổi
                if (newState.priority < currentLayerStates[StateLayer.FullBody].priority)
                {
                    Debug.Log($"Blocked {newState.stateName} by FullBody state");
                    return;
                }
            }

            // Exit old state
            previousState?.Exit();

            // Set new state
            currentLayerStates[layer] = newState;

            // Play animation trên đúng layer
            if (newState.animationClip != null)
            {
                animancerLayers[layer].Play(newState.animationClip, newState.crossFadeDuration);
            }

            // Enter new state
            newState.Enter();

            // Nếu là FullBody, override tất cả layer khác
            if (layer == StateLayer.FullBody)
            {
                OnFullBodyStateEnter(newState);
            }

            OnStateChanged?.Invoke(layer, newState);
        }

        private void OnFullBodyStateEnter(StateConfigSO fullBodyState)
        {
            // Tăng weight full body layer
            animancerLayers[StateLayer.FullBody].SetWeight(1f);

            // Giảm weight các layer khác
            foreach (var layer in animancerLayers)
            {
                if (layer.Key != StateLayer.FullBody)
                {
                    // Fade out các layer khác
                    StartCoroutine(FadeLayerWeight(layer.Key, 0f, 0.3f));
                }
            }
        }

        private System.Collections.IEnumerator FadeLayerWeight(StateLayer layer, float targetWeight, float duration)
        {
            float startWeight = animancerLayers[layer].Weight;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                animancerLayers[layer].SetWeight(Mathf.Lerp(startWeight, targetWeight, t));
                yield return null;
            }

            animancerLayers[layer].SetWeight(targetWeight);
        }

        public void ClearFullBodyState()
        {
            if (currentLayerStates.TryGetValue(StateLayer.FullBody, out var fullBodyState))
            {
                fullBodyState.Exit();
                currentLayerStates.Remove(StateLayer.FullBody);

                // Fade out full body layer
                StartCoroutine(FadeLayerWeight(StateLayer.FullBody, 0f, 0.3f));

                // Fade in các layer khác
                foreach (var layer in animancerLayers)
                {
                    if (layer.Key != StateLayer.FullBody)
                    {
                        StartCoroutine(FadeLayerWeight(layer.Key, 1f, 0.3f));
                    }
                }
            }
        }

        private bool HasFullBodyState()
        {
            return currentLayerStates.ContainsKey(StateLayer.FullBody) &&
                   currentLayerStates[StateLayer.FullBody] != null;
        }

        protected  void Update()
        {
            // Kiểm tra transitions cho từng layer
            foreach (var layerStates in statesByLayer)
            {
                StateLayer layer = layerStates.Key;

                // Skip nếu fullbody state đang active
                if (HasFullBodyState() && layer != StateLayer.FullBody)
                    continue;

                // Lấy state hiện tại của layer
                if (!currentLayerStates.ContainsKey(layer))
                    continue;

                StateConfigSO currentState = currentLayerStates[layer];

                // Kiểm tra các transition
                foreach (var state in layerStates.Value)
                {
                    if (state != currentState && state.IsMatchingCondition())
                    {
                        SetLayerState(layer, state);
                        break; // Chỉ chuyển 1 state mỗi frame
                    }
                }
            }
        }

        // Helper methods
        public StateConfigSO GetLayerState(StateLayer layer)
        {
            return currentLayerStates.TryGetValue(layer, out var state) ? state : null;
        }

        public bool IsInState(StateConfigSO state)
        {
            return currentLayerStates.ContainsValue(state);
        }

        // Public methods cho external code
        public void PlayUpperBodyState(StateConfigSO state)
        {
            SetLayerState(StateLayer.UpperBody, state);
        }

        public void PlayFullBodyState(StateConfigSO state)
        {
            SetLayerState(StateLayer.FullBody, state);
        }
    }

}