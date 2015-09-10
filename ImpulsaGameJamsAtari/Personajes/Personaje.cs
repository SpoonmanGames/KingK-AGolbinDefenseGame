using ImpulsaGameJamsAtari.Personajes.Behaviors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.StateControl;
using System;
using System.Collections.Generic;

namespace ImpulsaGameJamsAtari.Personajes
{
    public enum EstadoPersonaje
    {
        Quieto,
        Caminando,
        Atacando,
        Muerto
    }

    public abstract class Personaje
    {
        #region Propiedades y Atributos

        #region Constantes

        const int VelocidadBase = 5;
        const float VidaBase = 100;
        const float AtaqueBase = 20;

        public const int NivelArmaBase = 1;

        #endregion

        #region Prop

        /// <summary>
        /// Obtener el estado actual del personaje.
        /// </summary>
        public EstadoPersonaje EstadoPersonaje { get; set; }

        /// <summary>
        /// Permite cambiar la vida del personaje.
        /// </summary>
        public float Vida { get { return this.vida; } set { this.vida = value; } }
        float vida;

        /// <summary>
        /// Permite saber o cambiar el ataque que hace este personaje.
        /// </summary>
        public float Ataque { get { return this.ataque; } set { this.ataque = value; } }
        float ataque;

        /// <summary>
        /// Obtiene la textura completa del Personaje.
        /// </summary>
        protected Texture2D Textura { get { return this.textura; } set { this.textura = value; } }
        Texture2D textura;

        /// <summary>
        /// Nombre de la Textura.
        /// </summary>
        string nombreTextura;
        
        /// <summary>
        /// Porción de la Textura que se dibujará.
        /// </summary>
        protected Rectangle Sprite { get { return this.spriteRect; } set { this.spriteRect = value; } }
        Rectangle spriteRect;

        /// <summary>
        /// Permite obtener la caja de colisión del Personaje
        /// </summary>
        public virtual Rectangle BoundingBox
        {
            get
            {
                if(EstadoPersonaje != EstadoPersonaje.Muerto)
                    return new Rectangle((int)Posicion.X, (int)Posicion.Y, (int)Ancho, (int)Altura);
                else
                    return new Rectangle(0, 0, 0, 0);
            }   
        }


        /// <summary>
        /// Permite cambiar la escala del tamaño de la textura que se dibujará.
        /// </summary>
        protected Vector2 Escala { get { return this.vectorEscala; } set { this.vectorEscala = value; } }
        Vector2 vectorEscala;        

        /// <summary>
        /// Contador del frame actual dibujandose.
        /// </summary>
        protected int FrameActual { get { return this.frameActual; } set { this.frameActual = value; } }
        int frameActual;

        /// <summary>
        /// Tiempo al que se dibujan los frames.
        /// </summary>
        protected int FPS { get { return this.framePorSegundo; } set { this.framePorSegundo = value; } }
        int framePorSegundo;

        /// <summary>
        /// Vector que nos permite saber y asignar la posición del personaje.
        /// </summary>
        public Vector2 Posicion { get { return this.posicion; } set { this.posicion = value; } }
        Vector2 posicion;

        /// <summary>
        /// Permite obtener la altura del personaje.
        /// </summary>
        public float Altura { get { return Sprite.Height * Escala.Y; } }

        /// <summary>
        /// Permite obtener la ancho del personaje.
        /// </summary>
        public float Ancho { get { return Sprite.Width * Escala.X; } }

        /// <summary>
        /// Permite obtener el tiempo transcurrido en el juego.
        /// </summary>
        protected float TiempoTranscurrido { get { return this.tiempoTranscurrido; } set { this.tiempoTranscurrido = value; } }
        float tiempoTranscurrido;

        /// <summary>
        /// Permite obtener y asignar el ancho de un Frame.
        /// </summary>
        protected int FrameWidth { get { return this.frameWidth; } set { this.frameWidth = value; } }
        int frameWidth;

        protected int FrameHight { get { return this.framwHight; } set { this.framwHight = value; } }
        int framwHight;

        /// <summary>
        /// Permite obtener y asignar la posición Y desde donde se leerán los frames desde el SpriteMap
        /// </summary>
        protected int FrameYPosition { get { return this.frameYPosition; } set { this.frameYPosition = value; } }
        int frameYPosition;

        /// <summary>
        /// Permite obtener y asignar cuantos frames hay en una animación.
        /// </summary>
        protected int FrameLimit { get { return this.frameLimit; } set { this.frameLimit = value; } }
        int frameLimit;

        /// <summary>
        /// Permite saber si una animación a terminado completamente o no.
        /// </summary>
        protected bool AnimacionTerminada { get { return this.animacionTerminada; } set { this.animacionTerminada = value; } }
        bool animacionTerminada;

        /// <summary>
        /// Permite obtener y asignar la velocidad del personaje.
        /// </summary>
        public int Velocidad { get { return this.velocidad; } set { this.velocidad = value; } }
        int velocidad;

        /// <summary>
        /// Nos permite saber si el personaje ha sido atacado o no.
        /// </summary>
        protected bool Atacado { get { return this.atacado; } set { this.atacado = value; } }
        bool atacado;

        public int ArmaduraBase { get { return this.armaduraBase; } set { this.armaduraBase = value; } }
        int armaduraBase;

        /// <summary>
        /// Nos permite saber si las acciones se aplican hacia la izquierda o no
        /// </summary>
        protected bool OrientaciónIzquierda { get; set; }

        public Color Color { get; set; }

        public bool IsMuerto { 
            get {
                return this.EstadoPersonaje == EstadoPersonaje.Muerto && listaDanoRecibido.Count == 0 ? true : false;
            }
        }

        public int ContadorMuertes { get; set; }

        public int DanoCorrectnessPosition { get; set; }

        /// <summary>
        /// Guarda una referencia al nivel donde existirá el personaje
        /// </summary>
        public Nivel Nivel { get; private set; }

        public int NivelPersonaje { get; set; }      

        protected SpriteEffects SpriteEffect;
        protected int YOffsetBarraVida;

        #endregion

        #region At
        
          
        float backoffIntencity;

        // Para mostrar el dano
        List<MostrarDano> listaDanoRecibido = new List<MostrarDano>();
        float danoDelay;
        SpriteFont Font;
        TimeSpan danoTime;
        float danoUpVelocity;

        // Para mostrar barra de energía
        Texture2D barraVida;
        float anchoBarra;

        // Control de pantalla
        int offSetBordes;

        ContentManager content;

        #endregion

        #endregion        

        public Personaje(Nivel nivel, string nombreTextura)
        {
            this.Nivel = nivel;

            this.Color = Color.White;
            this.armaduraBase = 1;
            this.offSetBordes = 10;
            this.YOffsetBarraVida = 3;
            this.danoTime = TimeSpan.FromSeconds(0.5);
            this.danoUpVelocity = 10;
            this.anchoBarra = Vida;
            this.danoDelay = 1000;
            this.backoffIntencity = 1;
            this.atacado = false;
            this.EstadoPersonaje = EstadoPersonaje.Quieto;
            this.nombreTextura = nombreTextura;
            this.vida = VidaBase;
            this.ataque = AtaqueBase;
            this.posicion = Vector2.Zero;
            this.textura = null;
            this.spriteRect = new Rectangle();
            this.NivelPersonaje = 1;
            this.ContadorMuertes = 0;
            this.velocidad = VelocidadBase;
            this.frameActual = 0;
            this.framePorSegundo = 60;
            this.vectorEscala = Vector2.One;
            this.SpriteEffect = SpriteEffects.None;
            this.tiempoTranscurrido = 0f;
            this.frameWidth = 0;
            this.frameYPosition = 0;
            this.frameLimit = 0;
            this.AnimacionTerminada = false;            
        }

        public virtual void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null)
        {
            if (content == null && !tutorial) {
                content = new ContentManager(Nivel.ScreenManagerController.Game.Services, "Content");
                this.Font = this.Nivel.ScreenManagerController.Font;
            } else {
                content = new ContentManager(screenManagerController.Game.Services, "Content");
                this.Font = screenManagerController.Font;
            }
            this.textura = content.Load<Texture2D>(String.Format("Texturas/{0}", nombreTextura));
            
            this.barraVida = content.Load<Texture2D>("Texturas/blank");            
        }

        public virtual void UnloadContent() {
            content.Unload();
        }

        #region Update 

        public virtual void Update(GameTime gameTime) 
        {
            TiempoTranscurrido += gameTime.ElapsedGameTime.Milliseconds;

            UpdateAtacado(gameTime);

            switch (EstadoPersonaje)
            {
                case EstadoPersonaje.Quieto:
                    UpdateQuieto();                    
                    break;
                case EstadoPersonaje.Caminando:
                    UpdateCaminando();
                    break;
                case EstadoPersonaje.Atacando:
                    UpdateAtacando();
                    break;
                case EstadoPersonaje.Muerto:
                    UpdateMuerto();  
                    
                    break;
            }

            UpdateFrames();

            Sprite = new Rectangle(FrameActual * FrameWidth, FrameYPosition, FrameWidth, FrameHight);
        }

        private void UpdateFrames() {
            if (TiempoTranscurrido > 1000f / FPS) {
                ++FrameActual;
                TiempoTranscurrido = 0f;

                if (FrameActual >= FrameLimit) {
                    if (EstadoPersonaje == EstadoPersonaje.Atacando || EstadoPersonaje == EstadoPersonaje.Muerto) {
                        AnimacionTerminada = true;
                        --FrameActual;
                    } else {
                        FrameActual = 0;
                    }
                }
            }
        }

        protected virtual void UpdateAtacado(GameTime gameTime){
            if (atacado)
            {
                // Tiempo de la animación de retroseso
                TimeSpan time = TimeSpan.FromSeconds(0.5);

                float updateOffset = (float)gameTime.ElapsedGameTime.TotalMilliseconds /
                    time.Milliseconds;

                backoffIntencity -= updateOffset;

                float backoffPosition = (float)Math.Pow(backoffIntencity, 2);

                float fuerzaIzq;
                if (OrientaciónIzquierda)
                    fuerzaIzq = Posicion.X - backoffPosition * 3;
                else
                    fuerzaIzq = Posicion.X + backoffPosition * 3;

                    Posicion = new Vector2(fuerzaIzq, Posicion.Y);

                if (backoffIntencity <= 0)
                {
                    backoffIntencity = 1;
                    this.atacado = false;
                }
            }
        }
        
        public virtual void UpdateQuieto() { }

        public virtual void UpdateCaminando() { }

        public virtual void UpdateAtacando() { }

        public virtual void UpdateMuerto() { }

        #endregion       

        public bool SiendoAtacado(float dano){

            this.Atacado = true;
            this.Vida -= dano;

            listaDanoRecibido.Add(new MostrarDano(dano.ToString("0"), this.danoUpVelocity));

            if (this.Vida <= 0)
            {
                this.EstadoPersonaje = EstadoPersonaje.Muerto;
                FrameActual = 0;
                return true;
            }

            return false;
        }

        public virtual void HandleInput(InputState input) { }

        public virtual void Draw(GameTime gameTime, bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null, float transitionAlpha = 1f)
        {
            SpriteBatch spriteBatch;

            if(!tutorial)
                spriteBatch = Nivel.ScreenManagerController.SpriteBatch;
            else
                spriteBatch = screenManagerController.SpriteBatch;

            spriteBatch.Draw(
                textura,
                posicion,
                spriteRect,
                this.Color * transitionAlpha,
                0f,
                Vector2.Zero,
                vectorEscala,
                SpriteEffect,
                0f
            );

            if (!tutorial) {
                DibujarMuestrasDeDano(gameTime, spriteBatch);
                DibujarBarraDeVida(spriteBatch);
            }
        }

        private void DibujarMuestrasDeDano(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (listaDanoRecibido.Count > 0)
            {

                foreach (MostrarDano dano in listaDanoRecibido)
                {
                    dano.DanoTransition = (float)gameTime.ElapsedGameTime.Milliseconds / danoTime.Milliseconds;

                    if (dano.Alpha >= 1)
                    {
                        dano.DanoOutputTime += gameTime.ElapsedGameTime.Milliseconds;

                        if (dano.DanoOutputTime > danoDelay)
                        {
                            dano.IsInverso = true;
                            dano.DanoOutputTime = 0;
                        }
                    }

                    if (dano.IsInverso) {
                        dano.DanoTransition *= -1;
                        if(dano.Alpha == 0)
                            dano.Terminado = true;
                    }

                    dano.Alpha += dano.DanoTransition;
                    dano.Velocidad += Math.Abs(dano.DanoTransition) * 4;
                    dano.Alpha = MathHelper.Clamp(dano.Alpha, 0, 1);

                    spriteBatch.DrawString(
                        this.Font,
                        dano.GetDano,
                        new Vector2(this.posicion.X, this.posicion.Y - DanoCorrectnessPosition - dano.Velocidad),
                        Color.White * dano.Alpha
                    );
                }
                listaDanoRecibido.RemoveAll(d => d.Terminado);
            }
        }

        private void DibujarBarraDeVida(SpriteBatch spriteBatch)
        {
            //int posicionBarraVida = (int)(Posicion.X + offsetBarra);
            //if (SpriteEffect == SpriteEffects.FlipHorizontally)
                //posicionBarraVida -= offsetBarra * 2;

            spriteBatch.Draw(
                barraVida,
                new Rectangle((int)(Posicion.X), (int)Posicion.Y - YOffsetBarraVida, (int)Vida / 3, 5),
                Color.White
            );
        }

        /// <summary>
        /// Permite mover verticalmente el personaje
        /// </summary>
        /// <param name="izquierda">Por defecto se mueve a la derecha. Cambiar a true para mover a la izquierda.</param>
        public virtual void MovimientoHorizontal(bool izquierda = false)
        {
            if (izquierda)
            {
                this.posicion.X -= this.velocidad;
                SpriteEffect = SpriteEffects.None;

                this.posicion.X = Math.Max(this.posicion.X, offSetBordes);
            }
            else
            {
                this.posicion.X += this.velocidad;
                SpriteEffect = SpriteEffects.FlipHorizontally;

                this.posicion.X = Math.Min(this.posicion.X, Nivel.ScreenManagerController.GraphicsDevice.Viewport.Width - Ancho - offSetBordes);
            }
                
        }
    }
}
