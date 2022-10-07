using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class CubePrefab : MonoBehaviour
    {
        public TextMeshProUGUI textMeshProUGUI;
        public int animationSpeed = 20;

        public int value;

        private readonly Color[] textColors =
        {
            new(4, 4, 4),
            new(242, 235, 226)
        };

        private readonly List<Color> cubeColors = new()
        {
            new Color(238 / 255f, 228 / 255f, 218 / 255f),
            new Color(238 / 255f, 228 / 255f, 218 / 255f),
            new Color(237 / 255f, 224 / 255f, 200 / 255f),
            new Color(242 / 255f, 177 / 255f, 121 / 255f),
            new Color(245 / 255f, 149 / 255f, 99 / 255f),
            new Color(246 / 255f, 124 / 255f, 95 / 255f),
            new Color(246 / 255f, 94 / 255f, 59 / 255f),
            new Color(237 / 255f, 207 / 255f, 114 / 255f),
            new Color(237 / 255f, 204 / 255f, 97 / 255f),
            new Color(237 / 255f, 200 / 255f, 80 / 255f),
            new Color(237 / 255f, 200 / 255f, 80 / 255f),
            new Color(237 / 255f, 194 / 255f, 46 / 255f),
            new Color(60 / 255f, 58 / 255f, 50 / 255f)
        };

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetState(Cube state)
        {
            var changed = state.Value != value;
            value = state.Value;
            if (state.IsZero)
            {
                Visible(false);
                return;
            }

            textMeshProUGUI.text = state.Value.ToString();
            textMeshProUGUI.color = state.Value <= 4 ? textColors[0] : textColors[1];
            var geometricSeqToIndex = (int)(Mathf.Log(state.Value, 2) - 1);
            geometricSeqToIndex = geometricSeqToIndex > cubeColors.Count - 1 ? cubeColors.Count - 1 : geometricSeqToIndex;
            meshRenderer.material.color = cubeColors[geometricSeqToIndex];
            if (!changed) return;
            StartCoroutine(ScaleAnimate());
            IEnumerator<WaitForSeconds> ScaleAnimate()
            {
                // increase size
                transform.localScale *= 1.02f;
                yield return new WaitForSeconds(0.1f);
                transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            }
        }

        public void Visible(bool b)
        {
            meshRenderer.enabled = b;
            textMeshProUGUI.enabled = b;
        }

        public void CreateMoveAnimation(CubePrefab targetCube, bool hideTarget)
        {
            var currentCube = transform;
            var toMove = Vector3.Distance(currentCube.position, targetCube.transform.position);
            var copy = CreateCopy();
            Visible(false);
            StartCoroutine(AnimateCube());

            GameBoard.AnimationCount++;
            IEnumerator AnimateCube()
            {
                var target = targetCube.transform.position;
                if (hideTarget)
                {
                    targetCube.Visible(false);
                }
                while (true)
                {
                    if (copy.transform.position.Equals(target))
                    {
                        GameBoard.AnimationCount--;
                        if (hideTarget)
                        {
                            targetCube.Visible(true);
                        }                            
                        Destroy(copy);
                        yield break;
                    }

                    copy.transform.position = Vector3.MoveTowards(copy.transform.position, target, Time.deltaTime * (animationSpeed * toMove));
                    yield return null;
                }
            }

            GameObject CreateCopy()
            {
                var instantiate = Instantiate(gameObject, transform.parent, true);
                var cubePrefab = GetComponent<CubePrefab>();
                cubePrefab.Visible(true); // Make sure it is visible. Sometimes it is not visible for some reason
                instantiate.transform.position += new Vector3(0, 0, 0.01f); // move copy a bit back to not overlap text with others
                return instantiate;
            }
        }
    }
}