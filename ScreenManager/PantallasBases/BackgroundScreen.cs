using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.StateControl;
using System;

namespace ScreenManager.PantallasBases
{
    /// <summary>
    /// Clase usada para dibujar imagenes de fondo. Esta clase debe ser siempre llamada antes
    /// de una pantalla principal para asegurar que quedé en el fondo.
    /// </summary>
    public class BackgroundScreen : GameScreen
    {
        #region Fields


        /// <summary>
        /// Para manejar el contenido cargado por esta screen
        /// </summary>
        ContentManager content;


        /// <summary>
        /// Para setear la imagen del background
        /// </summary>
        Texture2D backgroundTexture;

        /// <summary>
        /// Nombre de la imagen del background
        /// </summary>
        string background_name;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor, define los tiempos de transición
        /// </summary>
        /// <param name="background_name">Nombre de la imagen que se usará de fondo.</param>
        public BackgroundScreen(string background_name)
        {
            this.background_name = background_name;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Load & Unload Content

        /// <summary>
        /// Permite cargar el graphic content de esta pantalla.
        /// De esta forma el contenido puede ser Unloaded después de forma
        /// independiente, evitando así que el contenido de esta Screen siga presente
        /// incluso después de que esta Screen desaparesca.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>(background_name);
        }


        /// <summary>
        /// Unload todo el contenido cargado por esta Screen
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update & Draw

        /// <summary>
        /// A diferencia de otras Screen, esta pantalla no debe hacer TransitionOff
        /// cuando es tapada por otra. La idea es que siempre se mantenga en el fondo.
        /// Por ello el override fuerza o engaña al update para que nunca sepa
        /// que tiene otra Screen tapandola.
        /// Esto también permite que Background tenga animación inclusive si tiene otras Screen
        /// encima.
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        /// <param name="otherScreenHasFocus">Dice si otra Screen tiene el foco</param>
        /// <param name="coveredByOtherScreen">Dice si esta Screen está tapada por otra</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Dibuja la Screen.
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Draw(GameTime gameTime)
        {
            //Obtiene el spriteBatch del ScreenManager
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            Viewport viewport = ScreenManagerController.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha)
                             );

            spriteBatch.End();
        }

        #endregion
    }
}
