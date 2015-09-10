using ImpulsaGameJamsAtari.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ImpulsaGameJamsAtari.Personajes
{
    /// <summary>
    /// BUG: si el nivel sube y sube el goblin ya no cae en la pantalla
    /// y los humanos pueden pasar de largo
    /// </summary>
    public partial class Goblin : Personaje
    {
        public readonly int ID;
        Texture2D select;
        ContentManager content;

        public Goblin(Nivel nivel, int id, string nombreTextura)
            : base(nivel, nombreTextura)
        {
            this.ID = id;
            this.OrientaciónIzquierda = true;
            this.FrameHight = 22;
            SpriteEffect = SpriteEffects.FlipHorizontally;
            DanoCorrectnessPosition = 30;
        }

        public override void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null)
        {
            base.LoadContent(tutorial, screenManagerController);

            if (content == null && !tutorial)
                content = new ContentManager(Nivel.ScreenManagerController.Game.Services, "Content");
            else
                content = new ContentManager(screenManagerController.Game.Services, "Content");            

            Escala = new Vector2(2, 2);
            Sprite = new Rectangle(0, 0, 15, 22);

            select = content.Load<Texture2D>("Texturas/selectHover");
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();
        }

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
            FrameLimit = 5;
            AnimacionTerminada = true;
        }

        public override void UpdateAtacando()
        {
            FrameWidth = 25;
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

        public bool ColisionHumana(bool atacando = false)
        {
            foreach (Humano humano in Nivel.humanosInvadiendo) {
                if (humano.BoundingBox.Intersects(this.BoundingBox)) {
                    this.Posicion = new Vector2(Posicion.X - 1, Posicion.Y);

                    if (atacando) {
                        //golpeando al humano
                        Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe2);
                    }

                    if (atacando)
                        if (humano.SiendoAtacado(this.Ataque + (NivelArmaBase * Forja.Forja.NivelArma) - humano.ArmaduraBase)) {
                            this.ContadorMuertes++;
                            Nivel.Herramientas += humano.Herramientas;

                            if (this.ContadorMuertes % 2 == 0 && this.NivelPersonaje < 7) {
                                Sonido.PlayEffect(Sound.PiezasEfectos.LevelUp);

                                ++this.NivelPersonaje;                                

                                float incremento =  (float)(10 * this.NivelPersonaje) /100.0f;

                                this.Vida += this.Vida * incremento;
                                this.Escala = new Vector2(this.Escala.X * ( 1.0f + incremento), this.Escala.Y * (1.0f + incremento));

                                this.Posicion = new Vector2(
                                    this.Posicion.X,
                                    (float)Nivel.ScreenManagerController.GraphicsDevice.Viewport.Height - 100.0f - this.Altura
                                );

                                float alphaColor = (float)(this.NivelPersonaje - 1) / 6.0f;
                                this.Color = new Color(this.Color.R - (int)(255 * alphaColor), this.Color.G, this.Color.B - (int)(255 * alphaColor), 255);
                            }

                            //Muerte humano
                            Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Muerte3);
                        }


                    return true;
                }   
            }

            return false;
        }

        public bool ColisionGoblinRey(bool atacando = false) {
            if (Nivel.goblinRey.BoundingBox.Intersects(this.BoundingBox)) {
                this.Posicion = new Vector2(Posicion.X + 1, Posicion.Y);

                return true;
            }

            return false;
        }

        public bool ColisionForja() {
            if (Nivel.forja.BoundingBox.Intersects(this.BoundingBox)) {
                return true;
            }

            return false;
        }


        public override void HandleInput(ScreenManager.StateControl.InputState input)
        {
            if (!Atacado) {
                if (input.IsLeft(null)) {
                    EstadoPersonaje = EstadoPersonaje.Caminando;
                    if (!ColisionHumana() && !ColisionGoblinRey())
                        this.MovimientoHorizontal(true);
                } else if (input.IsRight(null)) {
                    EstadoPersonaje = EstadoPersonaje.Caminando;
                    if (!ColisionHumana())
                        this.MovimientoHorizontal();
                } else if (input.IsA(null)) {
                    Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe);
                    EstadoPersonaje = EstadoPersonaje.Atacando;
                    AnimacionTerminada = false;
                    FrameActual = 0;
                    ColisionHumana(true);
                } else if (input.IsD(null)) {
                    if (Forja.Forja.TrampasDisponibles > 0) {

                        float posx;
                        if (SpriteEffect == SpriteEffects.None) {
                            posx = this.Posicion.X - 25;
                        }else{
                            posx = this.Posicion.X + Ancho + 5;
                        }

                        Nivel.AddPincho(posx);
                        --Forja.Forja.TrampasDisponibles;
                    }
                } else if (AnimacionTerminada) {
                    EstadoPersonaje = EstadoPersonaje = EstadoPersonaje.Quieto;
                }

                if (input.IsEnter(null)) {
                    if (ColisionForja()) {
                        Nivel.ScreenManagerController.AddScreen(
                            Nivel.forja, null
                        );
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, bool seleccionado, bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null, float transitionAlpha = 1f)
        {
            base.Draw(gameTime, tutorial, screenManagerController, transitionAlpha);

            if (seleccionado && !IsMuerto)
            {
                SpriteBatch spriteBatch;
                
                if(!tutorial)
                    spriteBatch = Nivel.ScreenManagerController.SpriteBatch;
                else
                    spriteBatch = screenManagerController.SpriteBatch;

                spriteBatch.Draw(
                    select,
                    new Rectangle(
                        (int)(Posicion.X + select.Width / 2),
                        (int)(Posicion.Y - 20),
                        (int)(select.Width * Escala.X),
                        (int)(select.Height * Escala.Y)
                    ),
                    Color.White * transitionAlpha
                );
            }
        }
    }
}
