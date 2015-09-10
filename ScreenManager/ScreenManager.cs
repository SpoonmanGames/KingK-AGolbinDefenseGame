using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.StateControl;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScreenManager
{
    /// <summary>
    /// ScreenManager es un componente que maneja una o más instancias de GameScreen.
    /// Las mantiene en una cola y realiza Update y Draw én el orden en que están guardadas.
    /// Mantiene también el input en la pantalla correcta y en standby las pantallas que no están
    /// activas en un momento dado.
    /// <remarks>Esta es la clase principal del proyecto, se extiende de DrawableGameComponent por
    /// lo que debe ser implementada al cmienzo de un proyecto Monogame.</remarks>
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields 

        //Lista de Screens contenida en este ScreenManager
        List<GameScreen> screens = new List<GameScreen>();

        //Lista de Screens a Actualizar
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        //Para tener un mapa de los controles en cada pantalla
        InputState input = new InputState();

        #endregion

        #region Properties

        /// <summary>
        /// Un SpriteBatch por defecto que será compartido para todas las Screens.
        /// Evita que cada screen tengan que crear su propia instancia local.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Un Font por defecto compartido para todas las Screens.
        /// Evita que cada screen tengan que crear su propia instancia local.
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// Engine de audio que permite manejar soundBanks y waveBanks
        /// basado en XACT
        /// </summary>
        public AudioEngine AudioEngine { get; private set; }

        /// <summary>
        /// Sistema de Debug: Si es true imprimirá por consola la lista de 
        /// pantallas que tiene guardad en un momento dado.
        /// </summary>
        public bool TraceEnabled { get; private set; }

        /// <summary>
        /// Permite saber si ScreenManager se ha inicializado y cargado sus componentes graficos.
        /// </summary>
        private bool IsInitialized { get; set; }

        /// <summary>
        /// Textura vacia que sirve para dibujar en ella una capa negra de transparencia
        /// </summary>
        private Texture2D BlankTexture { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor del ScreenManager
        /// </summary>
        /// <param name="game">Instancia del juego donde tendrá lugar este ScreenManager</param>
        public ScreenManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Inicializa los componentes del ScreenManager
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //Se han cargado los componentes graficos
            IsInitialized = true;
        }

        /// <summary>
        /// Carga los componente graficos por defecto para las demás Screns
        /// </summary>
        protected override void LoadContent()
        {
            //Para cargar el contenido que es exclusivo del ScreenManager
            ContentManager content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("Fonts/GameFont");
            BlankTexture = content.Load<Texture2D>("Texturas/blank");

            //Hace que cada una de las screens contenidas carguen sus content
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Descarga el contenido grafico de todas las screens contenidas
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update & Draw

        /// <summary>
        /// Permite que cada Screen realice el Update correspondiente
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Update(GameTime gameTime)
        {
            //Lee el input para las Screens
            this.input.Update();

            //Limpia la lista y la vuelve a llenar
            //Esto es para evitar errores de sincronización entre las listas
            screensToUpdate.Clear();
            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            //Setea de forma general los booleanos de control
            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            //Mientras exista alguna Screen que necesite Update
            while (screensToUpdate.Count > 0)
            {
                //Obtiene la Screen con mayor prioridad y la remueve
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                //Realiza el Update del screen obtenido
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                //Si la Screen está apareciendo o está activa
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    //Si esta pantalla es la que tiene el foco
                    //Le permite manejar el input
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    //Si esta Screen no es un Popup se debe informar a las siguientes pantallas
                    //de que serán tapadas por esta nueva pantalla
                    if (!screen.IsAPopup)
                        coveredByOtherScreen = true;
                }
            }
        }


        /// <summary>
        /// Hace que cada Screen se dibuje si es que no está en
        /// estado Hidden
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Añade una nueva Screen al ScreenManager
        /// </summary>
        /// <param name="screen">Screen a agregar</param>
        /// <param name="controllingPlayer">Player que controlará esta Screen, puede ignorarse</param>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManagerController = this;
            screen.IsExiting = false;

            //Si existe un GaphicDevice entonces cargará el content de la Screen
            if (IsInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }


        /// <summary>
        /// Remueve abruptamente una Screen del ScreenManager.
        /// Se recomienda usar GameScreen.ExitScreen previamente para
        /// que la Screen haga un TransitionOff
        /// </summary>
        /// <param name="screen">Screen ha remover</param>
        public void RemoveScreen(GameScreen screen)
        {
            //Si existe un GraphicDevice se descarga el contenido de la Screen
            if (IsInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }


        /// <summary>
        /// Retorna una copia de los Screens contenidos. La copia es por motivos de seguridad.
        /// Así solo se pueden agregar y quitar Screens del ScreenManager mediante
        /// el AddScreen y RemoveScreen
        /// </summary>
        /// <returns>Retorra la lista de screens en un array.</returns>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Dibuja un sprite de pantalla completa que es negro y se trasluce.
        /// Usado para hacer Fade In y Fade Out. También para oscurecer un poco 
        /// una Screen cuando existe un Popup
        /// </summary>
        /// <param name="alpha">Valor de traslucides</param>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            SpriteBatch.Draw(BlankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            SpriteBatch.End();
        }


        #endregion
    }
}
