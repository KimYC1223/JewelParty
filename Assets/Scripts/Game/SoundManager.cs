using System.Collections.Generic;
using UnityEngine;
using HexaBlast.Common;

namespace HexaBlast.Game
{
    public class SoundManager:MonoBehaviour
    {
        public AudioSource[] Sources;
        public AudioSource   MusicSource;

        public AudioClip BGM_Game;
        public AudioClip BGM_Lobby;

        public AudioClip SFX_Cant;
        public AudioClip SFX_Create;
        public AudioClip SFX_Pop;
        public AudioClip SFX_DontMove;
        public AudioClip SFX_Tada;
        public AudioClip SFX_Star;
        public AudioClip SFX_ReadyGo;
        public AudioClip SFX_Click;

        private static Stack<AudioClip> audioStack;

        private void Awake()
        {
            foreach (AudioSource source in Sources)
            {
                source.volume = 0.7f;
            }
        }

        private void OnDisable()
        {
            foreach ( AudioSource source in Sources )
            {
                source.Stop();
            }
        }

        public void PlaySound(SFX sfx)
        {
            AudioClip sound = null;
            switch ( sfx )
            {
                case SFX.CANT      : sound = SFX_Cant;     break;
                case SFX.POP       : sound = SFX_Pop;      break;
                case SFX.CREATE    : sound = SFX_Create;   break;
                case SFX.DONT_MOVE : sound = SFX_DontMove; break;
                case SFX.TADA      : sound = SFX_Tada;     break;
                case SFX.STAR      : sound = SFX_Star;     break;
                case SFX.READY_GO  : sound = SFX_ReadyGo;  break;
                case SFX.CLICK     : sound = SFX_Click;    break;
            }
            audioStack.Push(sound);
        }

        public void PlayBackgroundMusic(BGM bgm)
        {
            AudioClip sound = null;
            switch ( bgm )
            {
                case BGM.GAME  : sound = BGM_Game;  break;
                case BGM.LOBBY : sound = BGM_Lobby; break;
            }

            if( MusicSource.isPlaying == true )
            {
                MusicSource.Stop();
            }
            MusicSource.clip = sound;
            MusicSource.loop = true;
            MusicSource.Play();
        }

        private void Update()
        {
            if( audioStack == null)
            {
                audioStack = new Stack<AudioClip>();
            }
            while( audioStack.Count > 0)
            {
                AudioClip sound = audioStack.Pop();
                bool isPlay = false;
                for(int i = 0 ; i < Sources.Length ; i++ )
                {
                    if ( Sources[i].isPlaying == false )
                    {
                        Sources[i].clip = sound;
                        Sources[i].Play();
                        isPlay = true;
                        break;
                    }
                }
                if ( isPlay == false )
                {
                    audioStack.Push(sound);
                    break;
                }
            }
        }
    }
}
