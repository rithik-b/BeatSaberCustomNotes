﻿using Zenject;
using CustomNotes.Data;
using SiraUtil.Services;
using CustomNotes.Utilities;
using System;
using CustomNotes.Settings.Utilities;
using IPA.Loader;
using System.Linq;

namespace CustomNotes.Managers
{
    internal class CustomNoteManager : IInitializable
    {
        private readonly Submission _submission;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;
        private readonly PluginConfig _pluginConfig;
        private readonly IDifficultyBeatmap _level;

        internal CustomNoteManager([InjectOptional] Submission submission, NoteAssetLoader noteAssetLoader, GameplayCoreSceneSetupData gameplayCoreSceneSetupData, PluginConfig pluginConfig, IDifficultyBeatmap level)
        {
            _submission = submission;
            _noteAssetLoader = noteAssetLoader;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
            _pluginConfig = pluginConfig;
            _level = level;
        }
        public void Initialize()
        {
            if (_noteAssetLoader.SelectedNote != 0 && !Utils.IsNoodleMap(_level))
            {
                LayerUtils.CameraSet = false;
                CustomNote activeNote = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];

                // Some code to regulate bombs and the bomb patch
                if (activeNote.NoteBomb != null)
                {
                    MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteBomb);
                    LayerUtils.BombPatchRequired = false;
                }
                else if (activeNote.NoteBomb == null && (_pluginConfig.HMDOnly == true || LayerUtils.HMDOverride == true))
                {
                    LayerUtils.BombPatchRequired = true;
                }
                else LayerUtils.BombPatchRequired = false;

                if (_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes)
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Ghost Notes");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows)
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Disappearing Arrows");
                }
                if (Utils.IsNoodleMap(_level))
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Noodle Extensions");
                }
            }
        }
    }
}