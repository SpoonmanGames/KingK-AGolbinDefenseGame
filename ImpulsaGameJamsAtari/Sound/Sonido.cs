using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ImpulsaGameJamsAtari.Sound {    

    public enum PiezasMusica{
        Menu,
        CuevaIntro,
        CuevaLoop
    }

    public enum PiezasEfectos {
        Golpe,
        Golpe2,
        Golpe3,
        Golpe4,
        Muerte,
        Muerte2,
        Muerte3,
        IGIntro,
        SPIntro,
        LevelUp,
        PowerUp
    }

    public class Sonido {
        static List<Song> musica = new List<Song>();
        static List<bool> isMPlaying = new List<bool>();
        static List<SoundEffect> effectos = new List<SoundEffect>();
        static public bool Creado;

        ScreenManager.ScreenManager ScreenManagerController;
        static ContentManager content;

        public Sonido(ScreenManager.ScreenManager ScreenManager){
            ScreenManagerController = ScreenManager;
            Creado = true;
        }

        public void LoadContent() {
            if (content == null)
                content = new ContentManager(this.ScreenManagerController.Game.Services, "Content");

                musica.Add(content.Load<Song>("Sonidos/Musica/Menu-2"));
                musica.Add(content.Load<Song>("Sonidos/Musica/Fight-intro-2"));
                musica.Add(content.Load<Song>("Sonidos/Musica/Fight-loop-2"));

                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Golpe-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Golpe2-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Golpe3-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Golpe4-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Muerte-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Muerte2-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/Muerte3-2"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/ig-intro"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/sp-intro"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/level-up"));
                effectos.Add(content.Load<SoundEffect>("Sonidos/FX/power-up"));

                for (int i = 0; i < musica.Count; i++) {
                    isMPlaying.Add(false);
                }

                SoundEffect.MasterVolume = 1f;                    
        }

        public static void UnLoadContent() {
            content.Unload();
        }

        static public void PlayMusic(PiezasMusica piezaMusica) {

            MediaPlayer.Stop();
            switch (piezaMusica) {
                case PiezasMusica.Menu:
                    if (!isMPlaying[0]) {
                        MediaPlayer.Play(musica[0]);
                        MediaPlayer.IsRepeating = true;
                        isMPlaying[0] = true;
                        isMPlaying[1] = false;
                        isMPlaying[2] = false;
                    }
                    break;
                case PiezasMusica.CuevaIntro:
                    if (!isMPlaying[1]) {
                        MediaPlayer.Play(musica[1]);
                        MediaPlayer.IsRepeating = false;
                        isMPlaying[0] = false;
                        isMPlaying[1] = true;
                        isMPlaying[2] = false;
                    }       
                    break;
                case PiezasMusica.CuevaLoop:
                    if (!isMPlaying[2]) {
                        MediaPlayer.Play(musica[2]);
                        MediaPlayer.IsRepeating = true;
                        isMPlaying[0] = false;
                        isMPlaying[1] = false;
                        isMPlaying[2] = true;
                    }
                    break;
            }
        }

        static public bool IsMusicStop() {
            return MediaPlayer.State == MediaState.Stopped;
        }

        static public void PlayEffect(PiezasEfectos piezaEffecto) {
            switch (piezaEffecto) {
                case PiezasEfectos.Golpe:
                    effectos[0].Play();
                    break;
                case PiezasEfectos.Golpe2:
                    effectos[1].Play();
                    break;
                case PiezasEfectos.Golpe3:
                    effectos[2].Play();
                    break;
                case PiezasEfectos.Golpe4:
                    effectos[3].Play();
                    break;
                case PiezasEfectos.Muerte:
                    effectos[4].Play();
                    break;
                case PiezasEfectos.Muerte2:
                    effectos[5].Play();
                    break;
                case PiezasEfectos.Muerte3:
                    effectos[6].Play();
                    break;
                case PiezasEfectos.IGIntro:
                    effectos[7].Play();
                    break;
                case PiezasEfectos.SPIntro:
                    effectos[8].Play();
                    break;
                case PiezasEfectos.LevelUp:                    
                    effectos[9].Play(0.6f, 0.0f, 0.0f);
                    break;
                case PiezasEfectos.PowerUp:
                    effectos[10].Play(0.4f, 0.0f, 0.0f);
                    break;
            }
        }
    }
}
