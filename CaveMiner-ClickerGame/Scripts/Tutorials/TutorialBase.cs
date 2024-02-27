using System;
using System.Collections;
using UnityEngine;

namespace CaveMiner
{
    public class TutorialBase : MonoBehaviour
    {
        [SerializeField] private string _tutorialId;

        protected Action<string> _onCompleted;

        public string TutorialId => _tutorialId;

        public void StartTutorial(Action<string> onCompleted)
        {
            gameObject.SetActive(true);

            if (_onCompleted == null)
                _onCompleted = onCompleted;

            StartCoroutine(Tutorial());
        }

        protected virtual void CompleteTutorial()
        {
            _onCompleted?.Invoke(_tutorialId);
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator Tutorial()
        {
            yield return null;
        }

        protected IEnumerator WaitScreenTap()
        {
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
#if UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    yield return new WaitForFixedUpdate();
                    break;
                }
#else
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                    break;
#endif

                yield return null;
            }

            yield return new WaitForFixedUpdate();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}