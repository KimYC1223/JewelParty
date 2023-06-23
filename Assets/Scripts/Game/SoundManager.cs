using System.Collections.Generic;
using UnityEngine;
using MatchMatch.Common;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    //  사운드 재생을 담당하는 매니저
    //===============================================================================================================================================
    public class SoundManager:MonoBehaviour
    {
        #region FIELD
        public AudioSource[] Sources;           // 효과음을 재생하는 오디오 소스들
        public AudioSource   MusicSource;       // 음악을 재생하는 오디오 소스

        // Background Music
        public AudioClip BGM_Game;
        public AudioClip BGM_Lobby;

        // Sound FX
        public AudioClip SFX_Cant;
        public AudioClip SFX_Create;
        public AudioClip SFX_Pop;
        public AudioClip SFX_DontMove;
        public AudioClip SFX_Tada;
        public AudioClip SFX_Star;
        public AudioClip SFX_ReadyGo;
        public AudioClip SFX_Click;

        // 재생 Stack
        private static Stack<AudioClip> audioStack;
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  효과음을 재생하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 효과음을 재생하는 메서드. <br />
        /// <see cref="SFX"/>에 정의된 소리를 재생 할 수 있다.
        /// </summary>
        /// <param name="sfx">재생할 효과음</param>
        public void PlaySound(SFX sfx)
        {
            // enum 값에 따라 재생할 Sound 결정
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

        //===========================================================================================================================================
        //
        //  음악을 재생하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 음악을 재생하는 메서드. <br />
        /// <see cref="BGM"/>에 정의된 소리를 재생 할 수 있다.
        /// </summary>
        /// <param name="bgm">재생할 음악</param>
        public void PlayBackgroundMusic(BGM bgm)
        {
            // enum 값에 따라 재생할 BGM 결정
            AudioClip sound = null;
            switch ( bgm )
            {
                case BGM.GAME  : sound = BGM_Game;  break;
                case BGM.LOBBY : sound = BGM_Lobby; break;
            }

            // 만약 이미 재생중이라면, 취소함
            if( MusicSource.isPlaying == true )
            {
                MusicSource.Stop();
            }

            // 무한 반복하도록 플레이
            MusicSource.clip = sound;
            MusicSource.loop = true;
            MusicSource.Play();
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  시작 할 때, 볼륨을 설정함
        //
        //===========================================================================================================================================
        private void Awake()
        {
            // TO-DO : 오디오 볼륨 조정

            foreach ( AudioSource source in Sources )
            {
                source.volume = 0.7f;
            }
        }

        //===========================================================================================================================================
        //
        //  Disable 될 때, 재생되는 음악 및 소리 끄기
        //
        //===========================================================================================================================================
        private void OnDisable()
        {
            // 소리 재생 중단
            foreach ( AudioSource source in Sources )
            {
                source.Stop();
            }
            MusicSource.Stop();
        }

        //===========================================================================================================================================
        //
        //  매 프레임마다 호출되는 Update 메서드
        //
        //===========================================================================================================================================
        private void Update()
        {
            // Stack이 비어있을 경우, 초기화
            if( audioStack == null)
            {
                audioStack = new Stack<AudioClip>();
            }

            //=======================================================================================================================================
            //  만약 Stack이 비어있지 않은 경우, 소리를 출력함
            //=======================================================================================================================================
            while ( audioStack.Count > 0 )
            {
                // 재생할 소리를 가져옴
                AudioClip sound = audioStack.Pop();
                bool isPlay = false;
                
                // SFX 오디오 소스들을 돌면서 반복
                for(int i = 0 ; i < Sources.Length ; i++ )
                {
                    // 재생중이 아니라면, 플레이하고, Loop 탈출
                    if ( Sources[i].isPlaying == false )
                    {
                        Sources[i].clip = sound;
                        Sources[i].Play();
                        isPlay = true;
                        break;
                    }
                }
                
                // 만약 재생 못했으면, Stack에 도로 돌려놓고 While 탈출
                if ( isPlay == false )
                {
                    audioStack.Push(sound);
                    break;
                }
            }
        }
        #endregion
    }
}
