using CaveMiner.Secure;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine;

namespace CaveMiner
{
    public class GooglePlayGamesManager : Singleton<GooglePlayGamesManager>
    {
        public byte[] Data { get; private set; }

        private Action _onSavedGameDataReaded;
        private ISavedGameMetadata _savedGameMetadata;


        protected override void Awake()
        {
            base.Awake();

            PlayGamesPlatform.DebugLogEnabled = false;
            PlayGamesPlatform.Activate();
        }

        public void OpenSavedGame(string filename, Action onSavedGameDataReaded)
        {
            _onSavedGameDataReaded = onSavedGameDataReaded;

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }

        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            Debug.Log($"GooglePlayGamesManager - OnSavedGameOpened: {status}");

            if (status == SavedGameRequestStatus.Success)
            {
                _savedGameMetadata = game;
                LoadGameData(_savedGameMetadata);
            }
            else
            {
                // handle error
            }
        }

        private void LoadGameData(ISavedGameMetadata game)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
        }

        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {
            Debug.Log($"GooglePlayGamesManager - OnSavedGameDataRead: {status}");

            if (status == SavedGameRequestStatus.Success)
            {
                Data = data;
                _onSavedGameDataReaded?.Invoke();
            }
            else
            {
                // handle error
            }
        }

        public void SaveGame(byte[] savedData, int secondsPlaytime)
        {
            Debug.Log("Save Game CLOUD (GooglePlay)");

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            builder = builder.WithUpdatedPlayedTime(new TimeSpan(0, 0, secondsPlaytime));
            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            savedGameClient.CommitUpdate(_savedGameMetadata, updatedMetadata, savedData, OnSavedGameWritten);
        }

        public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            Debug.Log("Save Game CLOUD (GooglePlay) Completed: " + status);

            if (status == SavedGameRequestStatus.Success)
            {
                // handle reading or writing of saved game.
            }
            else
            {
                // handle error
            }
        }
    }
}