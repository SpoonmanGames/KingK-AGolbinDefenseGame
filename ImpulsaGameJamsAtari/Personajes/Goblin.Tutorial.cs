using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ImpulsaGameJamsAtari.Personajes {
    public partial class Goblin {

        ScreenManager.ScreenManager screenManagerController;
        bool izq;

        float attackDelayTutorial;
        float attackDelayMaxTutorial;

        public Goblin(ScreenManager.ScreenManager screenManager)
            : this(null, 1, "goblin-1"){
            this.screenManagerController = screenManager;
            izq = false;
            this.Velocidad = 2;
            attackDelayTutorial = 0f;
            attackDelayMaxTutorial = 500f;
        }

        public void LoadTutorial() {
            this.LoadContent(true, screenManagerController);
        }

        public void UpdateCaminandoTutorial(GameTime gameTime){
            this.EstadoPersonaje = Personajes.EstadoPersonaje.Caminando;

            this.Update(gameTime);

            if (Posicion.X <= 280)
                izq = false;

            if (Posicion.X >= 330)
                izq = true;

            MovimientoHorizontalTutorial(izq);
        }

        public void UpdateAtacandoTutorial(GameTime gameTime) {
            this.EstadoPersonaje = Personajes.EstadoPersonaje.Atacando;
            this.SpriteEffect = SpriteEffects.None;

            this.Update(gameTime);

            attackDelayTutorial += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameActual == 1)
                

                if (attackDelayTutorial >= attackDelayMaxTutorial) {
                    FrameActual = 0;
                    attackDelayTutorial = 0;
                }
        }

        public void DrawTutorial(GameTime gameTime, float transitionAlpha, bool seleccionado = false) {
            this.Draw(gameTime, seleccionado, true, screenManagerController, transitionAlpha);
        }

        public virtual void MovimientoHorizontalTutorial(bool izquierda = false) {
            if (izquierda) {
                Posicion = new Vector2(Posicion.X - Velocidad, Posicion.Y);
                SpriteEffect = SpriteEffects.None;

                Posicion = new Vector2(Math.Max(Posicion.X, 280), Posicion.Y);
            } else {
                Posicion = new Vector2(Posicion.X + Velocidad, Posicion.Y);
                SpriteEffect = SpriteEffects.FlipHorizontally;

                Posicion = new Vector2(Math.Min(Posicion.X, 330), Posicion.Y);
            }

        }
    }
}
