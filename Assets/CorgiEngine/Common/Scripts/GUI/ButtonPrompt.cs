using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    public class ButtonPrompt : MonoBehaviour
    {
        [Header("Bindings")]
        public SpriteRenderer Border;
        public SpriteRenderer Background;
        public CanvasGroup TextCanvasGroup;
        public Text PromptText;

        [Header("Durations")]
        public float FadeInDuration = 0.2f;
        public float FadeOutDuration = 0.2f;

        protected Color _alphaZero = new Color(1f, 1f, 1f, 0f);
        protected Color _alphaOne = new Color(1f, 1f, 1f, 1f);

        public virtual void SetText(string newText)
        {
            PromptText.text = newText;
        }

        public virtual void SetBackgroundColor(Color newColor)
        {
            Background.color = newColor;
        }

        public virtual void SetTextColor(Color newColor)
        {
            PromptText.color = newColor;
        }

        public virtual void Show()
        {
            StartCoroutine(MMFade.FadeSprite(Border, FadeInDuration, _alphaOne));
            StartCoroutine(MMFade.FadeSprite(Background, FadeInDuration, _alphaOne));
            StartCoroutine(MMFade.FadeCanvasGroup(TextCanvasGroup, FadeInDuration, 1f, true));
        }

        public virtual void Hide()
        {
            StartCoroutine(HideCo());
        }

        protected virtual IEnumerator HideCo()
        {
            StartCoroutine(MMFade.FadeSprite(Border, FadeOutDuration, _alphaZero));
            StartCoroutine(MMFade.FadeSprite(Background, FadeOutDuration, _alphaZero));
            StartCoroutine(MMFade.FadeCanvasGroup(TextCanvasGroup, FadeOutDuration, 0f, true));
            yield return new WaitForSeconds(0.3f);
            this.gameObject.SetActive(false);
        }
    }
}