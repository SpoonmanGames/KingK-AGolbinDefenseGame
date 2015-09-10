using ImpulsaGameJamsAtari.Forja.Trampas;
using ImpulsaGameJamsAtari.Menus;
using ImpulsaGameJamsAtari.Personajes;
using ImpulsaGameJamsAtari.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ScreenManager.MenuScren;
using ScreenManager.PantallasBases;
using ScreenManager.StateControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpulsaGameJamsAtari
{
    /// <summary>
    /// BUG: En el modo infinito al reiniciarse las oleadas desaparecen los soldados vivos
    /// </summary>
    public class Nivel : GameScreen
    {
        public List<Goblin> goblins = new List<Goblin>();
        public GoblinRey goblinRey;
        public List<Humano> humanosInvadiendo = new List<Humano>();
        public List<Humano> humanos = new List<Humano>();
        public Forja.Forja forja;
        public List<Trampa> trampas = new List<Trampa>();
        GUI gui;

        int numeroGoblins;
        int numeroHumanos;
        int selectedGoblin;

        Texture2D suelo;
        Rectangle rectSuelo;

        Texture2D letreroKK;
        Rectangle rectLetreroKK;

        ContentManager content;

        public int Herramientas;

        //oleadas mms
        int dificultad;
        int oleadaActual;
        float oleadaTimeDelay;
        float oleadaMaxdelay;
        int indexHumanosInvocados;

        float delayMsg;
        float delayMaxMsg;

        bool onloop;
        bool modoInfinito;

        public Nivel(int dificultad)
        {
            onloop = false;

            delayMsg = 0f;
            delayMaxMsg = 3000f;

            this.dificultad = dificultad;
            this.modoInfinito = false;

            if (this.dificultad >= 6) {
                this.dificultad = 1;
                this.modoInfinito = true;
            }

            oleadaActual = 1;
            oleadaTimeDelay = 12000f;
            oleadaMaxdelay = 15000f;
            indexHumanosInvocados = 0;

            Herramientas = 0;
            numeroGoblins = 5;
            numeroHumanos = 12;
            selectedGoblin = numeroGoblins - 1;

            for (int i = 0; i < numeroGoblins; i++)
            {
                goblins.Add(new Goblin(this, i, "goblin-1"));
            }
            goblinRey = new GoblinRey(this, "rey-1");

            for (int i = 0; i < numeroHumanos; i++) {
                humanos.Add(new Humano(this, "humano-1"));
            }

            forja = new Forja.Forja(this, string.Empty);

            gui = new GUI(this);            
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            suelo = content.Load<Texture2D>("Texturas/blank");
            rectSuelo = new Rectangle(
                0, 
                ScreenManagerController.GraphicsDevice.Viewport.Height - 100,
                ScreenManagerController.GraphicsDevice.Viewport.Width,
                100
            );

            foreach (Goblin goblin in goblins)
            {
                goblin.LoadContent();
                goblin.Posicion = new Vector2(
                    80 * goblin.ID + 200,
                    ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - goblin.Altura
                );
            }


            goblinRey.LoadContent();
            goblinRey.Posicion = new Vector2(
                25,
                ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - goblinRey.Altura
            );

            letreroKK = content.Load<Texture2D>("Texturas/KingKLetrero");
            rectLetreroKK = new Rectangle(
                0,
                0,
                letreroKK.Width,
                letreroKK.Height
            );

            foreach (Humano humano in humanos) {
                humano.LoadContent();
                humano.Posicion = new Vector2(
                    ScreenManagerController.GraphicsDevice.Viewport.Width + 0,
                    ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - humano.Altura
                );    
            }

            forja.Load();
            forja.Posicion = new Vector2(
                185,
                ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - forja.Altura
            );

            gui.LoadContent();
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();

            for (int i = 0; i < goblins.Count; i++)
            {
                goblins[i].UnloadContent();		 
            }

            goblinRey.UnloadContent();

            for (int i = 0; i < humanosInvadiendo.Count; i++) {
                humanosInvadiendo[i].UnloadContent();
            }

            for (int i = 0; i < humanos.Count; i++) {
                humanos[i].UnloadContent();
            }

            forja.UnLoad();

            for (int i = 0; i < trampas.Count; i++) {
                trampas[i].UnloadContent();
            }

            gui.UnloadContent();
        }

        /// <summary>
        /// BUG: Al cambiar de goblin este queda en su último estado.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            if (IsActive) {
                if (input.IsSpace(null)) {
                    HandleSelectGoblin();
                }

                if (input.IsEscape(null)) {
                    PopupScreen salir = new PopupScreen("Está seguro que quiere salir? o.ó ");

                    salir.Accepted += end_Accepted;

                    ScreenManagerController.AddScreen(salir, null);
                }

                foreach (Goblin goblin in goblins) {
                    if (goblin.ID == selectedGoblin)
                        if (goblin.EstadoPersonaje != EstadoPersonaje.Muerto)
                            goblin.HandleInput(input);
                        else {
                            HandleSelectGoblin();
                        }
                }
            }            
        }

        private void end_Accepted(object sender, PlayerIndexEventArgs e) {
            MediaPlayer.Stop();
            LoadingScreen.Load(ScreenManagerController, true, string.Empty, null, new BackgroundScreen("Texturas/mainmenu"), new MenuPrincipal(string.Empty));
        }

        void HandleSelectGoblin() {
            ++selectedGoblin;

            if (selectedGoblin >= numeroGoblins)
                selectedGoblin = 0;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Musica
            if (!onloop && Sonido.IsMusicStop()) {
                onloop = true;
                Sonido.PlayMusic(PiezasMusica.CuevaLoop);
            }

            if (IsActive) {

                oleadaTimeDelay += gameTime.ElapsedGameTime.Milliseconds;

                //Goblin
                foreach (Goblin goblin in goblins) {
                    goblin.Update(gameTime);
                }

                //Goblin Rey
                goblinRey.Update(gameTime);

                //Humano
                foreach (Humano humano in humanosInvadiendo) {
                    humano.Update(gameTime);
                }

                //Trampas
                foreach (Trampa trampa in trampas) {
                    trampa.Update(gameTime);    
                }                

                //Gui                
                gui.Update(
                    this.Herramientas,
                    Forja.Forja.NivelArma, 
                    Forja.Forja.NivelArmadura,
                    Forja.Forja.TrampasDisponibles,
                    goblins.Sum(g => g.ContadorMuertes) + trampas.Sum(t => t.ContadorMuertes),
                    oleadaActual - 1,
                    (int)(oleadaMaxdelay - oleadaTimeDelay) / 1000,
                    this.dificultad
                );

                if (oleadaTimeDelay >= oleadaMaxdelay) {
                    if (this.modoInfinito || oleadaActual <= 5) {
                        if (oleadaActual == 6) {
                            ++this.dificultad;
                            oleadaActual = 1;
                            AgregarHumanos();
                        }

                        GenerarOleada(oleadaActual, this.dificultad);
                        ++oleadaActual;
                        oleadaTimeDelay = 0f;                        
                    }
                }

                if (!this.modoInfinito && VerificaVictoria()) {
                    delayMsg += gameTime.ElapsedGameTime.Milliseconds;

                    if (delayMsg >= delayMaxMsg) {
                        PopupScreen victoria = new PopupScreen(
                            "Viva el Rey King K.!!!! El rey del nuevo mundo",
                            "\nPresione <ESC> para continuar", true);

                        victoria.Accepted += end_Accepted;
                        victoria.Cancelled += end_Accepted;

                        ScreenManagerController.AddScreen(victoria, null);
                    }
                } else if (VerificaDerrota()) {
                    delayMsg += gameTime.ElapsedGameTime.Milliseconds;

                    if (delayMsg >= delayMaxMsg) {
                        PopupScreen derrota = new PopupScreen(
                            "El Rey King. K. ha muerto... sucios humanos!",
                            "\nPresione <ESC> para continuar", true);

                        derrota.Accepted += end_Accepted;
                        derrota.Cancelled += end_Accepted;

                        ScreenManagerController.AddScreen(derrota, null);
                    }
                }
            }
        }

        void GenerarOleada(int numOleada, int dificultad) {

            int numHumanosAInvocar = Fibonacci(numOleada);

            for (int i = 0; i < numHumanosAInvocar; i++) {
                Humano hum = humanos[indexHumanosInvocados];

                hum.Posicion = new Vector2(
                    ScreenManagerController.GraphicsDevice.Viewport.Width + 20 + 30*i,
                    ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - hum.Altura
                );

                hum.ArmaduraBase += dificultad - 1;
                hum.Ataque += (dificultad - 1) * (float)Math.Log(dificultad, 2);
                hum.Velocidad *= (int)Math.Log(dificultad + 1, 2);
                hum.Vida += 10f * (float)Math.Log(dificultad, 2);

                float alphaColor = (float)(dificultad - 1) / 4.0f;
                hum.Color = new Color(hum.Color.R, hum.Color.G - (int)(255 * alphaColor), hum.Color.B - (int)(255 * alphaColor), 255);

                humanosInvadiendo.Add(hum);
                ++indexHumanosInvocados;
            }
        }

        int Fibonacci(int n) {
            if(n == 0)
                return 0;
            if(n == 1)
                return 1;

            return Fibonacci(n-1) + Fibonacci(n-2);
        }

        void AgregarHumanos() {

            for (int i = 0; i < numeroHumanos; i++) {
                Humano hum = new Humano(this, "humano-1");
                hum.LoadContent();
                hum.Posicion = new Vector2(
                    ScreenManagerController.GraphicsDevice.Viewport.Width + 20,
                    ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - hum.Altura
                );

                humanos.Add(hum);
            }
        }

        bool VerificaVictoria() {

            if(humanosInvadiendo.Count == numeroHumanos){
                foreach (Humano humano in humanosInvadiendo) {
                    if (humano.EstadoPersonaje != EstadoPersonaje.Muerto) {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        bool VerificaDerrota() {
            if (this.goblinRey.EstadoPersonaje == EstadoPersonaje.Muerto) {
                return true;
            }

            return false;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //FORJA
            forja.Dibujo();

            //GOBLINS
            foreach (Goblin goblin in goblins) {
                bool seleccionado = false;

                if (goblin.ID == selectedGoblin)
                    seleccionado = true;

                goblin.Draw(gameTime, seleccionado);
            }

            //REY GOBLIN
            goblinRey.Draw(gameTime);

            //LETRERO KK
            spriteBatch.Draw(
                letreroKK,
                new Vector2(
                (int)goblinRey.Posicion.X,
                (int)goblinRey.Posicion.Y - 10 - letreroKK.Height),
                rectLetreroKK,
                Color.White,
                0f,
                Vector2.Zero,                
                new Vector2(0.5f, 0.5f),
                SpriteEffects.None,
                0f
            );

            //HUMANO
            foreach (Humano humano in humanosInvadiendo) {
                humano.Draw(gameTime);
            }

            //TRAMPAS
            foreach (Trampa trampa in trampas) {
                trampa.Draw(gameTime);
            }            

            //GUI
            gui.Draw();

            //SUELO
            spriteBatch.Draw(suelo, rectSuelo, new Color(128, 128, 0, 255));  

            spriteBatch.End();
        }

        public void AddPincho(float posx){
            Pinchos pinchoD = new Pinchos(this, "pinchos-1");

            pinchoD.LoadContent();

            pinchoD.Posicion = new Vector2(
                posx,
                pinchoD.AlturaLimite + (pinchoD.Altura) - 5
            );
            pinchoD.AlturaInicial = (int)pinchoD.Posicion.Y;

            trampas.Add(pinchoD);
        }
    }
}
