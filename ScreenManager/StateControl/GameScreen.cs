using Microsoft.Xna.Framework;
using System;

namespace ScreenManager.StateControl
{
    /// <summary>
    /// Enum para saber el estado de transición del Screen
    /// <list type="bullet">
    /// <item> 
    /// <description>TransitionOn: Cuando la Screen está comenzando a verse.</description> 
    /// </item> 
    /// <item> 
    /// <description>Active: Cuando la Screen está activa.</description> 
    /// </item>
    /// <item> 
    /// <description>TransitionOff: Cuando la Screen comienza a irse.</description> 
    /// </item>
    /// <item> 
    /// <description>Hidde: Cuando la pantalla se ha ido pero sigue cargada para volver a aparecer.</description> 
    /// </item>
    /// </list>
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// Una Screen es un layer único que puede hacer Update y Draw.
    /// Es combinable con otros layers mediante el ScreenManager Class.
    /// </summary>
    public abstract class GameScreen
    {
        #region Properties

        #region Control


        /// <summary>
        /// Una Screen puede ser una nueva pantalla completamente o bien
        /// un simple popup, en este caso las pantallas que van quedando
        /// atras no deben desactivarse.
        /// </summary>
        public bool IsAPopup
        {
            get { return isAPopup; }
            protected set { isAPopup = value; }
        }
        private bool isAPopup = false;


        /// <summary>
        /// Una pantalla puede TransitionOff por dos razones:
        /// Puede quedar en standy by para permitir que otra Screen se dibuje sobre ella.
        /// O puede ser que este dejando el juego totalmente, por lo que debe ser eliminada despues
        /// de hacer TransitionOff.
        /// Si esta variable es true, la Screen será removida según el segundo caso enunciado.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }
        private bool isExiting = false;


        /// <summary>
        /// Revisa si esta pantalla es la pantalla activa en este momento,
        /// por lo que el usuario está interactuando con ella.
        /// </summary>
        public bool IsActive
        {
            get
            {
                /* Si ninguna otra pantalla tiene el foco y
                 * la pantalla está actualmente "apareciendo" (TransitionOn) o
                 * la pantalla está actualmente activa
                 */
                return !otherScreenHasFocus &&
                    (screenState == ScreenState.TransitionOn ||
                    screenState == ScreenState.Active);
            }
        }
        private bool otherScreenHasFocus;

        #endregion

        #region Transition


        /// <summary>
        /// Indica cuanto tiempo le toma hacer TransitionOn
        /// cuando la Screen ha sido activada.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }
        private TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// Indica cuanto tiempo le toma hacer TransitionOff
        /// cuando la Screen ha sido desactivada
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }
        private TimeSpan transitionOffTime = TimeSpan.Zero;


        /// <summary>
        /// Obtiene la posición/estado actual de la transición de la pantalla.
        /// Va de 0(pantalla activa sin transición) a 1(transición en su maximo punto).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }
        private float transitionPosition = 1;


        /// <summary>
        /// Obtiene el alpha(nivel de transparencia) del Screen.
        /// Va de 1(pantalla activa sin transición) a 0(pantalla en su máximo punto)
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        #endregion

        #region Referencias

        /// <summary>
        /// Obtiene el estado actual de la transición de la Screen.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }
        private ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// Obtiene el ScreenManager que maneja esta Screen, de modo de acceder al contexto de ella.
        /// </summary>
        public ScreenManager ScreenManagerController
        {
            get { return screenManagerController; }
            internal set { screenManagerController = value; }
        }
        private ScreenManager screenManagerController;

        /// <summary>
        /// Obtiene el index del player que está controlando la Screen actualmente.
        /// Si es NULL todos los player pueden controlar la Screen.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }
        private PlayerIndex? controllingPlayer;

        #endregion

        #endregion

        #region Methods To Override

        #region Load & Unload Content

        /// <summary>
        /// Carga los graphics content para la Screen
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Descarga los graphics content para la Screen
        /// </summary>
        public virtual void UnloadContent() { }

        #endregion

        #region HandleInput & Draw

        /// <summary>
        /// Permite que las Scren puedan usar diferentes input.
        /// Este metodo debe ser desactivado para la screen cuando no esté activada(IsActive == false)
        /// </summary>
        /// <param name="input">Clase input que permite mapear el teclado.</param>
        public virtual void HandleInput(InputState input) { }

        /// <summary>
        /// Llamada para dibujar la Screen en pantalla
        /// </summary>
        /// <param name="gameTime">Clase que guarda dinamicamente el tiempo transcurrido.</param>
        public virtual void Draw(GameTime gameTime) { }

        #endregion

        #endregion

        #region Update & UpdateTransition

        /// <summary>
        /// Permite que la pantalla se actualice, actualiza las transiciones On y Off.
        /// Este metodo es siempre llamado, de forma que la pantalla se mantenga en contexto
        /// en cualquier estado en el que se encuentre (Active, Hidde o Transition)
        /// </summary>
        /// <param name="gameTime">Tiempo del juego</param>
        /// <param name="otherScreenHasFocus">boleano para determinar si esta pantalla tiene el foco o no</param>
        /// <param name="coveredByOtherScreen">boleano para determinar si está siendo tapada por otra Screen</param>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Actualiza si la Screen tiene el foco en ese momento o no
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                //La Screen morira, por lo que debe tener una transición de salida
                screenState = ScreenState.TransitionOff;

                //Cuando la transición termine se debe remover la Screen
                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    ScreenManagerController.RemoveScreen(this);
                }
            }
            //Si la pantalla es tapada por otra, debe hacer transitionOff
            else if (coveredByOtherScreen)
            {
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    screenState = ScreenState.TransitionOff;
                }
                //Cuando la Screen termine la transición
                else
                {
                    screenState = ScreenState.Hidden;
                }
            }
            //En este caso la screen debe hacer un TransitionOn y volverse activa
            else
            {
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    screenState = ScreenState.TransitionOn;
                }
                //Terminó el TransitionOn
                else
                {
                    screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// Actualiza la posición de la transición, true si aún está realizando la transición.
        /// </summary>
        /// <param name="gameTime">Tiempo del juego</param>
        /// <param name="time">Intervalo de tiempo para la transición</param>
        /// <param name="direction">1 para realizar transitionOff, -1 para transitionOn</param>
        /// <returns>Retorna verdadero si aun no termina de actualizarse o false en caso contrario.</returns>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            //Guarda cuando queda para realizar la transición
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                //GameTime.ElapsedGametime tiene el tiempo transcurrido en cada update/frame
                //Esto dividido por el tiempo total que debe tomar la transición nos da el rango de avance del efecto
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            //Actualiza la posición de transición en la dirección dada
            transitionPosition += transitionDelta * direction;

            //Comprueba si se ha terminado de realizar la transición
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                //Fija transitionPosition entre 0 y 1 en caso de que haya superado estos limites
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            //Retorna verdadero si aún no ha terminado
            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Le dice a la pantalla que debe irse. A diferencia de screenManager.RemoveScreen
        /// este método realiza una transitionOff antes de eliminar la Screen. No la elimina
        /// inmediatamente.
        /// </summary>
        public void ExitScreen()
        {
            //Si el tiempo de transición de la Screen es 0 se elimina inmediatamente
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManagerController.RemoveScreen(this);
            }
            //Sino, se asigna true a IsExiting, para que se realice la transición adecuada
            else
            {
                IsExiting = true;
            }
        }

        #endregion
    }
}
