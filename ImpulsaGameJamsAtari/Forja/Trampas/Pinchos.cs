using ImpulsaGameJamsAtari.Personajes;
using Microsoft.Xna.Framework;

namespace ImpulsaGameJamsAtari.Forja.Trampas {
    public partial class Pinchos : Trampa {

        float Ataque;
        float attackDelay;
        float maxAttackDelay;

        bool activo;
        int velocidad;        
        
        float retracDelay;
        float maxRetracDelay;
        bool isRetracting;

        public int AlturaInicial { get; set; }
        public int AlturaLimite { get; set; }

        public Pinchos(Nivel nivel, string nombreTextura)
            : base(nivel, nombreTextura) {
                this.Nivel = nivel;
                this.isRetracting = false;
                this.activo = false;
                this.velocidad = 10;                
                this.Ataque = 30f;
                this.attackDelay = 3000f;
                this.maxAttackDelay = 3000f;
                this.retracDelay = 0f;
                this.maxRetracDelay = 500f;
                this.Escala = new Vector2(2, 2);                
        }

        public override void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null) {
            base.LoadContent();

            this.AlturaLimite = Nivel.ScreenManagerController.GraphicsDevice.Viewport.Height - 100 - this.Altura;
        }

        public override void Update(GameTime gameTime) {
            attackDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (attackDelay >= maxAttackDelay) {
                this.activo = true;
                ColisionHumana();
                attackDelay = 0f;
                this.isRetracting = false;
            }

            if (this.activo) {
                Activar(gameTime);
            }
        }

        void ColisionHumana() {
            foreach (Humano humano in Nivel.humanosInvadiendo) {
                if (humano.BoundingBox.Intersects(this.BoundingBox)) {

                        //golpeando al humano
                        Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Golpe2);

                        if (humano.SiendoAtacado(this.Ataque - humano.ArmaduraBase)) {                            
                            Nivel.Herramientas += humano.Herramientas;
                            ++this.ContadorMuertes;

                            //Muerte humano
                            Sound.Sonido.PlayEffect(Sound.PiezasEfectos.Muerte3);
                        }
                }
            }
        }

        void Activar(GameTime gameTime) {
            if(!isRetracting){
                this.Posicion = new Vector2(this.Posicion.X, this.Posicion.Y - this.velocidad);

                if (this.Posicion.Y <= this.AlturaLimite) {
                    this.Posicion = new Vector2(this.Posicion.X, this.AlturaLimite);
                }
            }

            retracDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (retracDelay >= maxRetracDelay) {
                this.isRetracting = true;
                this.Posicion = new Vector2(this.Posicion.X, this.Posicion.Y + this.velocidad);                

                if (this.Posicion.Y >= AlturaInicial) {
                    this.Posicion = new Vector2(this.Posicion.X, AlturaInicial);
                    this.activo = false;
                    retracDelay = 0f;
                }
            }
        }
    }
}
