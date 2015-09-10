using Microsoft.Xna.Framework;

namespace ImpulsaGameJamsAtari.Forja.Trampas {
    public partial class Pinchos {

        ScreenManager.ScreenManager screenManagerController;

        public Pinchos(ScreenManager.ScreenManager screenManager)
            : this(null, "pinchos-1"){
            this.screenManagerController = screenManager;
        }

        public void LoadTutorial() {
            base.LoadContent(true, screenManagerController);

            this.AlturaLimite = (int)this.Posicion.Y - this.Altura;
        }

        public void UpdateTutorial(GameTime gameTime) {
            attackDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (attackDelay >= maxAttackDelay) {
                this.activo = true;
                attackDelay = 0f;
                this.isRetracting = false;
            }

            if (this.activo) {
                Activar(gameTime);
            }
        }

        public void DrawTutorial(GameTime gameTime, float transitionAlpha) {
            this.Draw(gameTime, true, screenManagerController, transitionAlpha);
        }
    }
}
