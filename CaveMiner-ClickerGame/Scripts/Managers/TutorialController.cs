using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CaveMiner
{
    public class TutorialController : Singleton<TutorialController>
    {
        public bool TutorialPlaying { get; private set; }
        public Dictionary<string, TutorialBase> Tutorials = new Dictionary<string, TutorialBase>();
        public Action<string> onTutorialCompleted;

        private TutorialBase _currentTutorial;

        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneLoaded += (scene, loadSceneMode) => UpdateTutorialList();

            UpdateTutorialList();
        }

        public void CloseTutorial()
        {
            Debug.Log("Close Tutorial");

            TutorialPlaying = false;

            if (_currentTutorial != null)
                _currentTutorial.gameObject.SetActive(false);
        }

        private void UpdateTutorialList()
        {
            Debug.Log("Update Tutorial List");

            Tutorials.Clear();

            var allTutorials = FindObjectsOfType<TutorialBase>(true);
            foreach (var tutorial in allTutorials)
                Tutorials.Add(tutorial.TutorialId, tutorial);
        }

        public void StartTutorial(string tutorialId)
        {
            if (Tutorials.Count <= 0)
                UpdateTutorialList();

            Debug.Log("TutorialStart: " + tutorialId);

            TutorialPlaying = true;

            _currentTutorial = Tutorials[tutorialId];
            _currentTutorial.StartTutorial(OnTutorialCompleted);
        }

        private void OnTutorialCompleted(string tutorialId)
        {
            Debug.Log("TutorialCompleted: " + tutorialId);

            TutorialPlaying = false;
            _currentTutorial = null;

            onTutorialCompleted?.Invoke(tutorialId);
        }
    }
}