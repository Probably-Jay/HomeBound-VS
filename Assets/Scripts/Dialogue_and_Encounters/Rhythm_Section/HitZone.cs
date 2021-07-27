using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace NoteSystem
{

        /*
        public enum HitQuality
        {
            Miss
            , Early
            , Late
            , Good
            , Great
            , Perfect
        }
        */
        public class HitZone : MonoBehaviour
        {
            [SerializeField] List<WordNote> notesInChannel = new List<WordNote> { };
        Sprite normalSprite;
        SpriteRenderer sR;
        [SerializeField] Sprite hitSprite;
        [SerializeField] Sprite missSprite;
        private float animationTimer;

        // [SerializeField] KeyCode button = KeyCode.Space;
        // Start is called before the first frame update

        /*
        void ProcessHitOnNote(WordNote note, HitQuality quality)
        {
            note.Remove();
        }
        */
        private void Awake()
        {
            sR = this.GetComponent<SpriteRenderer>();
            normalSprite = sR.sprite;
        }
        private void Update()
        {
            if (animationTimer > 0.2)
            {
                sR.sprite = normalSprite;
            }
            else
            {
                animationTimer += Time.deltaTime;
            }
        }
        public void HitAnimation()
        {
            sR.sprite = hitSprite;
            animationTimer = 0;
        }
        public void MissAnimation()
        {
            sR.sprite = missSprite;
            animationTimer = 0;
        }

    }
}



