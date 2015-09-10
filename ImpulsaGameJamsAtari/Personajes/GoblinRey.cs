using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ImpulsaGameJamsAtari.Personajes
{
    /// <summary>
    /// BUG: Puede ser golpiado cuando muere.
    /// </summary>
    public class GoblinRey : Personaje
    {
        public override Rectangle BoundingBox {
            get {
                return new Rectangle((int)Posicion.X, (int)Posicion.Y, (int)Ancho - 5, (int)Altura);
            }
        }

        public GoblinRey(Nivel nivel, string nombreTextura)
            : base(nivel, nombreTextura)
        {
            this.Vida = 200;
            this.FrameHight = 20;
            SpriteEffect = SpriteEffects.FlipHorizontally;
            this.YOffsetBarraVida = 9;
            DanoCorrectnessPosition = 30;
        }

        public override void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null)
        {
            base.LoadContent(tutorial, screenManagerController);

            Escala = new Vector2(3, 3);
            Sprite = new Rectangle(0, 0, 20, 20);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void UpdateQuieto()
        {
            FrameHight = 20;
            FrameWidth = 20;
            TiempoTranscurrido = 0f;
            FrameYPosition = 0;
            FrameActual = 0;
            FrameLimit = 0;
            AnimacionTerminada = true;
        }

        public override void UpdateAtacando()
        {
            FrameHight = 26;
            FrameWidth = 20;
            FrameYPosition = 20;
            FPS = 10;
            FrameLimit = 2;
        }

        public override void UpdateMuerto()
        {
            FrameHight = 20;
            FrameWidth = 20;
            FrameYPosition = 0;
            FPS = 5;
            FrameLimit = 3;
        }

        protected override void UpdateAtacado(GameTime gameTime) {
            return;
        }

        public override void Draw(GameTime gameTime, bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null, float transitionAlpha = 1f)
        {
            base.Draw(gameTime);
        }
    }
}
