using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ImpulsaGameJamsAtari.Personajes
{
    public class Humano : Personaje
    {
        float TimeAttack;
        float delayAttack;

        public int Herramientas;

        public Humano(Nivel nivel, string nombreTextura)
            : base(nivel, nombreTextura) 
        {
            this.OrientaciónIzquierda = false;
            this.FrameHight = 22;
            this.Herramientas = 10;            
            this.ArmaduraBase = 0;
            DanoCorrectnessPosition = 10;
        }

        public override void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null)
        {
            base.LoadContent(tutorial, screenManagerController);

            delayAttack = 2000;
            TimeAttack = 2000f;
            Velocidad = 2;
            Escala = new Vector2(3, 3);
            Sprite = new Rectangle(0, 0, 15, 22);
        }

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TimeAttack += gameTime.ElapsedGameTime.Milliseconds;

            if(!Atacado)
            {
                if (EstadoPersonaje != EstadoPersonaje.Muerto)
                {
                    if (Nivel.goblinRey.IsMuerto) {
                        this.EstadoPersonaje = EstadoPersonaje.Caminando;
                        this.RetiradaVictoriosa();
                    } else {
                        if (!ColisionGoblin() && !ColisionGoblinRey() && this.AnimacionTerminada) {
                            this.EstadoPersonaje = EstadoPersonaje.Caminando;
                            this.MovimientoHorizontal(true);
                        } else {
                            if (TimeAttack >= delayAttack) {
                                if (AnimacionTerminada) {
                                    this.EstadoPersonaje = EstadoPersonaje.Atacando;
                                    this.AnimacionTerminada = false;
                                    FrameActual = 0;
                                    Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe4);
                                    ColisionGoblin(true);
                                    ColisionGoblinRey(true);
                                }

                                TimeAttack = 0f;
                            }
                        }
                    }
                }
            }
        }

        public override void UpdateQuieto()
        {
            FrameWidth = 15;
            TiempoTranscurrido = 0f;
            FrameYPosition = 0;
            FrameActual = 0;
            FrameLimit = 0;
            AnimacionTerminada = true;
        }

        public override void UpdateCaminando()
        {
            FrameWidth = 15;
            FrameYPosition = 0;
            FPS = 15;
            FrameLimit = 4;
            AnimacionTerminada = true;
        }

        public override void UpdateAtacando()
        {
            FrameWidth = 28;
            FrameYPosition = 22;
            FPS = 10;
            FrameLimit = 2;
        }

        public override void UpdateMuerto()
        {
            FrameWidth = 18;
            FrameYPosition = 44;
            FPS = 5;
            FrameLimit = 3;
        }

        #endregion

        bool ColisionGoblin(bool atacando = false)
        {
            foreach (Goblin gb in Nivel.goblins)
            {
                if (gb.BoundingBox.Intersects(this.BoundingBox))
                {
                    this.Posicion = new Vector2(Posicion.X + 0.1f, Posicion.Y);

                    if (atacando) {
                        Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe3);
                    }

                    if (atacando)
                        if (gb.SiendoAtacado(this.Ataque - (gb.ArmaduraBase * Forja.Forja.NivelArmadura))) {
                            this.ContadorMuertes++;

                            Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Muerte);
                        }

                    return true;
                }
            }

            return false;
        }

        public bool ColisionGoblinRey(bool atacando = false) {
            if (Nivel.goblinRey.BoundingBox.Intersects(this.BoundingBox)) {
                this.Posicion = new Vector2(Posicion.X + 1, Posicion.Y);

                if (atacando) {
                    Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe3);
                }

                if (atacando) {
                    if (Nivel.goblinRey.SiendoAtacado(this.Ataque - (Nivel.goblinRey.ArmaduraBase * Forja.Forja.NivelArmadura))) {
                        this.ContadorMuertes++;
                        Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Muerte2);
                    }
                }

                return true;
            }

            return false;
        }

        void RetiradaVictoriosa() {
            this.Posicion = new Vector2(this.Posicion.X + (float)this.Velocidad, this.Posicion.Y);
            SpriteEffect = SpriteEffects.FlipHorizontally;
        }

    }
}
