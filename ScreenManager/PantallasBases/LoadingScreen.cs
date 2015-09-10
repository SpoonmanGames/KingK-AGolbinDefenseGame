using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.StateControl;
using System;

namespace ScreenManager.PantallasBases
{
    /// <summary>
    /// Cordina la transición entre cambios de contextos.
    /// Usado cuando es necesario borrar todas las Screen de la cola y se necesit
    /// cargar otra pantalla con mucha carga. Para esto se hace:
    /// 
    /// <list type="bullet">
    /// <item> 
    /// <description>Le dice a todas las Scren que hagan TransitionOff.</description> 
    /// </item> 
    /// <item> 
    /// <description>Activa la Loading Screen.</description> 
    /// </item>
    /// <item> 
    /// <description>Loading Screen espera que la Screen previa terminé su transición.</description> 
    /// </item>
    /// <item> 
    /// <description>También espera que la siguiente screen en la cola cargue su contenido.</description> 
    /// </item>
    /// </list>     
    /// </summary>
    public class LoadingScreen : GameScreen
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

        /// <summary>
        /// Define si será una carga lenta o rápida
        /// </summary>
        bool loadingIsSlow;


        /// <summary>
        /// Permite identificar si las Screens antiguas ya se han sacado de la cola
        /// </summary>
        bool otherScreensAreGone;


        /// <summary>
        /// Guarda la lista de Screens a cargar
        /// </summary>
        GameScreen[] screensToLoad;

        #endregion

        #region Constructor


        /// <summary>
        /// El constructor es privado: la carga de pantallas debe ser
        /// hecha atravez del metodo estatico Load.
        /// </summary>
        /// <param name="loadingIsSlow">Booleano para definir si será una carga lenta</param>
        /// <param name="background_name">Nombre del fondo que tendrá esta pantalla. Puede ser vacio.</param>
        /// <parmaparam name="screensToLoad">Screen a cargar</parmaparam>
        private LoadingScreen(bool loadingIsSlow,
                                string background_name,
                                GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;
            this.background_name = background_name;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Activa la Loading Screen
        /// </summary>
        /// <param name="screenManager">Manager para agregar las nuevas pantallas a la cola</param>
        /// <param name="loadingIsSlow">Activa modo de cargado lento para poder visualizar bien la pantalla de Loading</param>
        /// <param name="background_name">Nombre del fondo que tendrá esta pantalla. Puede ser vacio.</param>
        /// <param name="controllingPlayer">Player que gatillo la acción</param>
        /// <param name="screensToLoad">Lista de Screens a mostrar despues de Loading Screen</param>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                string background_name,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {
            // Todas las Screens actuales deben hacer Transition Off
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Crea y activa la Loading Screen
            LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow,
                                                            background_name,
                                                            screensToLoad);

            // Añade Loading Screen a la cola de screens
            screenManager.AddScreen(loadingScreen, controllingPlayer);
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

            if (!background_name.Equals(String.Empty))
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

        #region Update and Draw


        /// <summary>
        /// Realiza el Update de la Screen
        /// </summary>
        /// <param name="gameTime">Para tener acceso al tiempo del juego</param>
        /// <param name="otherScreenHasFocus">Para saber si está Screen tiene el foco</param>
        /// <param name="coveredByOtherScreen">Para saber si esta pantalla está siendo tapada por otra</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                        bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Si toda las Screen previas en la cola han terminado
            // Se comienza a cargar las Screens
            if (otherScreensAreGone)
            {
                // Se remueve a si misma de la cola
                ScreenManagerController.RemoveScreen(this);

                // Realiza la carga secuencial de todas las Screen en la cola
                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManagerController.AddScreen(screen, ControllingPlayer);
                    }
                }

                /* Una ves terminado de cargar, use ResetElapsedTime para decirle al juego
                    * que se acaba de ejecutar un frame bien largo, así que no 
                    * intente de alcanzar el tiempo, mejor reiniciarse
                    */
                ScreenManagerController.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Dibuja la Loading Screen. Descrimina si es que la carga es lenta o es rapida.
        /// En el último caso no se muestra el mensaje LOADING...
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Draw(GameTime gameTime)
        {
            /* Si está Screen es la única en la cola entonces toda las otras Screen
                * ya han terminado. Por lo que después de dibujar bien la Loading Screen
                * se procede a realizar la carga de otras Screens.
                */
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManagerController.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            //Obtiene el spriteBatch del ScreenManager
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            Viewport viewport = ScreenManagerController.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            if (!background_name.Equals(String.Empty))
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

                spriteBatch.Draw(backgroundTexture, fullscreen,
                                 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha)
                                 );

                spriteBatch.End();
            }

            /* Si una Screen toma poco tiempo no es necesario mostrar un mensaje
                * de "Cargando..." ya que sería molesto ver la imagen aparecer tan brevemente.
                * Sin embargo, si sabemos que la pantalla tomará un poco más, la variable
                * loadingIsSlow permitirá tener el texto mostrado por una fracción de tiempo
                */
            if (loadingIsSlow)
            {
                SpriteFont font = ScreenManagerController.Font;

                const string message = "Loading...";

                // Centra el texto en el Viewport
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                // Para que se escriba con una Transition On/Off
                Color color = Color.White * TransitionAlpha;

                // Dibuja le texto
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

                spriteBatch.DrawString(font, message, textPosition, color);

                spriteBatch.End();
            }
        }


        #endregion
    }
}
