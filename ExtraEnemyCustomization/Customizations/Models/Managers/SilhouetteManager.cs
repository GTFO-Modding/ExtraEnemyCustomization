﻿using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Models.Managers
{
    public class EnemySilhouette : MonoBehaviour
    {
        public Material SilhouetteMaterial;

        private Color _LatestColorB = Color.clear;

        public EnemySilhouette(IntPtr ptr) : base(ptr)
        {

        }

        public void EnableSilhouette()
        {
            SetColorB(_LatestColorB);
        }
        public void DisableSilhouette()
        {
            SetColorB(Color.clear);
        }

        public void SetColorA(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorA", color);
        }

        public void SetColorB(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorB", color);
            _LatestColorB = color;
        }
    }

    public class SilhouetteManager : MonoBehaviour
    {
        public EnemyAgent _Agent;
        public Color _DefaultColor;
        public bool _RequireTag;
        public bool _ReplaceColorWithMarker;
        public bool _KeepOnDead;
        public float _DeadEffectDelay;

        private bool tagUpdateDone = true;
        private NavMarker enemyMarker = null;
        private EnemySilhouette[] silhouettes = null;

        public SilhouetteManager(IntPtr ptr) : base(ptr)
        {

        }

        internal void Start()
        {
            silhouettes = GetComponentsInChildren<EnemySilhouette>(true);
            foreach(var sil in silhouettes)
            {
                sil.SilhouetteMaterial = sil.GetComponent<Renderer>().material;
            }

            if (_RequireTag || _ReplaceColorWithMarker)
            {
                EnemyMarkerEvents.RegisterOnMarked(_Agent, OnMarked);
            }
            
            if (!_RequireTag)
            {
                SetColorB(_DefaultColor);
                Show();
            }
        }

        internal void Update()
        {
            if (!_Agent.Alive)
            {
                if (!_KeepOnDead)
                {
                    KillSilhouette(_DeadEffectDelay);
                    enabled = false;
                }
            }

            if (!tagUpdateDone)
            {
                if (!_Agent.IsTagged)
                {
                    if (_RequireTag)
                    {
                        Hide();
                    }

                    tagUpdateDone = true;
                }

                var alpha = enemyMarker?.m_enemySubObj?.m_sprites?[0]?.color.a ?? 0.0f;
                var tagColor = enemyMarker?.m_enemySubObj?.m_sprites?[0]?.color ?? _DefaultColor;
                if (_RequireTag)
                {
                    var newColor = _ReplaceColorWithMarker ? tagColor.AlphaMultiplied(alpha) : _DefaultColor.AlphaMultiplied(alpha);
                    SetColorB(newColor);
                }
                else
                {
                    var newColor = Color.Lerp(_DefaultColor, tagColor, alpha);
                    SetColorB(newColor);
                }
            }
        }

        public void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            enemyMarker = marker;
            tagUpdateDone = false;

            if (_RequireTag)
            {
                Show();
            }
        }

        public void KillSilhouette(float delay)
        {
            for (int i = 0; i < silhouettes.Length; i++)
            {
                CoroutineManager.BlinkOut(silhouettes[i].gameObject, delay);
            }
        }

        public void Show()
        {
            for (int i = 0; i < silhouettes.Length; i++)
            {
                silhouettes[i].EnableSilhouette();
            }
        }

        public void Hide()
        {
            for (int i = 0; i < silhouettes.Length; i++)
            {
                silhouettes[i].DisableSilhouette();
            }
        }

        public void SetColorA(Color color)
        {
            for (int i = 0; i < silhouettes.Length; i++)
            {
                silhouettes[i].SetColorA(color);
            }
        }

        public void SetColorB(Color color)
        {
            for (int i = 0; i < silhouettes.Length; i++)
            {
                silhouettes[i].SetColorB(color);
            }
        }
    }
}
