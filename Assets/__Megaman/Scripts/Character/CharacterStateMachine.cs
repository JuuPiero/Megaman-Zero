using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Megaman
{
    public enum StateLayer
    {
        Base = 0,
        UpperBody = 1,
        FullBody = 2
    }
    
    public class CharacterStateMachine : MonoBehaviour
    {
        [Header("States Configuration")]
        public List<StateConfigSO> allStates = new List<StateConfigSO>();

        [Header("Layer Settings")]
        public AnimancerComponent animancer;
        public AvatarMask upperBodyMask;
        
        [Header("Layer Count")]
        [SerializeField] private int maxLayers = 3;

        private Dictionary<StateLayer, AnimancerLayer> animancerLayers = new Dictionary<StateLayer, AnimancerLayer>();
        private Dictionary<StateLayer, StateConfigSO> currentLayerStates = new Dictionary<StateLayer, StateConfigSO>();
        private Dictionary<StateLayer, List<StateConfigSO>> statesByLayer = new Dictionary<StateLayer, List<StateConfigSO>>();

        public StateConfigSO BaseState => GetLayerState(StateLayer.Base);
        public StateConfigSO UpperBodyState => GetLayerState(StateLayer.UpperBody);
        public StateConfigSO FullBodyState => GetLayerState(StateLayer.FullBody);

        public System.Action<StateLayer, StateConfigSO> OnStateChanged;

        private void Awake()
        {
            if (animancer == null)
                animancer = GetComponentInChildren<AnimancerComponent>();

            EnsureLayersExist();
            SetupLayers();
            OrganizeStatesByLayer();
        }

        /// <summary>
        /// Đảm bảo Animancer có đủ số layers cần thiết
        /// </summary>
        private void EnsureLayersExist()
        {
            Debug.Log($"Current layers: {animancer.Layers.Count}, Need: {maxLayers}");

            // Thêm layers cho đến khi đủ số lượng
            while (animancer.Layers.Count < maxLayers)
            {
                // Add() không nhận parameter, nó tự tạo và return layer mới
                var newLayer = animancer.Layers.Add();
                Debug.Log($"Created new layer. Total: {animancer.Layers.Count}, Name: {newLayer}");
            }

            Debug.Log($"✓ Layers ready: {animancer.Layers.Count}");
        }

        protected void SetupLayers()
        {
            animancerLayers.Clear();

            // Base layer (index 0) - luôn tồn tại
            if (animancer.Layers.Count > 0)
            {
                animancerLayers[StateLayer.Base] = animancer.Layers[0];
                animancerLayers[StateLayer.Base].SetWeight(1f);
                Debug.Log($"✓ Base layer setup (index 0)");
            }

            // Upper body layer (index 1)
            if (animancer.Layers.Count > 1)
            {
                animancerLayers[StateLayer.UpperBody] = animancer.Layers[1];
                
                if (upperBodyMask != null)
                {
                    animancerLayers[StateLayer.UpperBody].SetMask(upperBodyMask);
                    Debug.Log($"✓ UpperBody layer setup (index 1) with mask: {upperBodyMask.name}");
                }
                else
                {
                    Debug.LogWarning("⚠ UpperBody layer has NO Avatar Mask!");
                }
                
                animancerLayers[StateLayer.UpperBody].SetWeight(1f);
            }

            // Full body override layer (index 2)
            if (animancer.Layers.Count > 2)
            {
                animancerLayers[StateLayer.FullBody] = animancer.Layers[2];
                animancerLayers[StateLayer.FullBody].SetWeight(0f);
                Debug.Log($"✓ FullBody layer setup (index 2)");
            }
        }

        protected void OrganizeStatesByLayer()
        {
            statesByLayer.Clear();
            
            foreach (StateLayer layer in System.Enum.GetValues(typeof(StateLayer)))
            {
                statesByLayer[layer] = new List<StateConfigSO>();
            }

            foreach (var state in allStates)
            {
                if (state != null)
                {
                    statesByLayer[state.stateLayer].Add(state);
                }
            }

            // Log kết quả
            foreach (var kvp in statesByLayer)
            {
                if (kvp.Value.Count > 0)
                {
                    string stateNames = string.Join(", ", kvp.Value.ConvertAll(s => s.stateName));
                    Debug.Log($"Layer {kvp.Key}: {kvp.Value.Count} states → [{stateNames}]");
                }
            }
        }

        public void Initialize(BaseCharacter owner, InputManager input)
        {
            foreach (var state in allStates)
            {
                state.Initialize(owner, animancer, input);
            }

            // Bắt đầu với state đầu tiên của mỗi layer
            foreach (var layerStates in statesByLayer)
            {
                if (layerStates.Value.Count > 0 && animancerLayers.ContainsKey(layerStates.Key))
                {
                    SetLayerState(layerStates.Key, layerStates.Value[0]);
                }
            }
        }

        public void SetLayerState(StateLayer layer, StateConfigSO newState)
        {
            if (newState == null)
            {
                Debug.LogError($"Cannot set null state on layer {layer}");
                return;
            }

            if (!animancerLayers.ContainsKey(layer))
            {
                Debug.LogError($"Layer {layer} not found!");
                return;
            }

            if (!statesByLayer[layer].Contains(newState))
            {
                Debug.LogError($"State '{newState.stateName}' doesn't belong to layer {layer}");
                return;
            }

            // Lấy state cũ
            StateConfigSO previousState = null;
            currentLayerStates.TryGetValue(layer, out previousState);

            // Kiểm tra FullBody conflict
            if (HasFullBodyState() && layer != StateLayer.FullBody)
            {
                var fullBodyState = currentLayerStates[StateLayer.FullBody];
                if (fullBodyState != null && newState.priority < fullBodyState.priority)
                {
                    Debug.Log($"🚫 Blocked '{newState.stateName}' by FullBody state");
                    return;
                }
            }

            // Exit old state
            previousState?.Exit();

            // Set new state
            currentLayerStates[layer] = newState;

            // Play animation trên đúng layer
            if (newState.animationClip != null && animancerLayers[layer] != null)
            {
                animancerLayers[layer].Play(newState.animationClip, newState.crossFadeDuration);
                Debug.Log($"▶ Playing '{newState.animationClip.name}' on layer {layer}");
            }

            // Enter new state
            newState.Enter();

            // FullBody override
            if (layer == StateLayer.FullBody)
            {
                OnFullBodyStateEnter(newState);
            }

            OnStateChanged?.Invoke(layer, newState);
        }

        private void OnFullBodyStateEnter(StateConfigSO fullBodyState)
        {
            if (!animancerLayers.ContainsKey(StateLayer.FullBody)) return;

            animancerLayers[StateLayer.FullBody].SetWeight(1f);

            foreach (var layer in animancerLayers)
            {
                if (layer.Key != StateLayer.FullBody)
                {
                    StartCoroutine(FadeLayerWeight(layer.Key, 0f, 0.3f));
                }
            }
        }

        private System.Collections.IEnumerator FadeLayerWeight(StateLayer layer, float targetWeight, float duration)
        {
            if (!animancerLayers.ContainsKey(layer)) yield break;

            float startWeight = animancerLayers[layer].Weight;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                if (animancerLayers.ContainsKey(layer))
                {
                    animancerLayers[layer].SetWeight(Mathf.Lerp(startWeight, targetWeight, t));
                }
                yield return null;
            }

            if (animancerLayers.ContainsKey(layer))
            {
                animancerLayers[layer].SetWeight(targetWeight);
            }
        }

        public void ClearFullBodyState()
        {
            if (currentLayerStates.TryGetValue(StateLayer.FullBody, out var fullBodyState))
            {
                fullBodyState.Exit();
                currentLayerStates.Remove(StateLayer.FullBody);

                if (animancerLayers.ContainsKey(StateLayer.FullBody))
                {
                    StartCoroutine(FadeLayerWeight(StateLayer.FullBody, 0f, 0.3f));
                }

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

        [ReadOnly, ShowInInspector] 
        public StateConfigSO currentState;
        
        protected virtual void Update()
        {
            foreach (var layerStates in statesByLayer)
            {
                StateLayer layer = layerStates.Key;

                if (HasFullBodyState() && layer != StateLayer.FullBody)
                    continue;

                if (!currentLayerStates.ContainsKey(layer))
                    continue;

                currentState = currentLayerStates[layer];

                foreach (var state in layerStates.Value)
                {
                    if (state != currentState && state.IsMatchingCondition())
                    {
                        SetLayerState(layer, state);
                        break;
                    }
                }
            }
        }

        public StateConfigSO GetLayerState(StateLayer layer)
        {
            return currentLayerStates.TryGetValue(layer, out var state) ? state : null;
        }

        public bool IsInState(StateConfigSO state)
        {
            return currentLayerStates.ContainsValue(state);
        }

        public void PlayUpperBodyState(StateConfigSO state)
        {
            SetLayerState(StateLayer.UpperBody, state);
        }

        public void PlayFullBodyState(StateConfigSO state)
        {
            SetLayerState(StateLayer.FullBody, state);
        }

        // Debug helpers
        [Button("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== Layer Debug ===");
            Debug.Log($"Animancer Layers: {animancer.Layers.Count}");
            
            for (int i = 0; i < animancer.Layers.Count; i++)
            {
                var layer = animancer.Layers[i];
                // Debug.Log($"Layer {i}: Weight={layer.Weight:F2}, Mask={(layer.Mask != null ? layer.Mask.name : "None")}");
            }
            
            Debug.Log($"\nCurrent States:");
            foreach (var kvp in currentLayerStates)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value?.stateName ?? "None"}");
            }
        }
    }
}