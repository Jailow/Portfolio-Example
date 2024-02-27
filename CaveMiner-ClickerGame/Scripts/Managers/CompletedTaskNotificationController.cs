using System.Collections.Generic;
using UnityEngine;
using CaveMiner.UI;

namespace CaveMiner
{
    public class CompletedTaskNotificationController : MonoBehaviour
    {
        private Queue<string> _queue = new Queue<string>();
        private UICompletedTaskNotification _notification;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _notification = GetComponentInChildren<UICompletedTaskNotification>(true);
            _notification.Init(OnAnimationCompleted);
        }

        private void OnAnimationCompleted()
        {
            TryShowNotification();
        }

        private void TryShowNotification()
        {
            if (_notification.IsPlaying || _queue.Count <= 0)
                return;

            if (_gameManager.GameState.SoundsIsOn)
            {
                var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                audioShot.Play(_gameManager.SoundData.TaskCompleted);
            }

            _notification.Show(_queue.Dequeue());
        }

        public void Show(string taskName)
        {
            _queue.Enqueue(taskName);

            TryShowNotification();
        }
    }
}