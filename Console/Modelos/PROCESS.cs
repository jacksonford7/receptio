namespace Console.ServicioConsole
{
    public partial class PROCESS
    {
        public string ImagenEstado
        {
            get
            {
                if (IS_OK)
                    return "/Imagenes/Finish.png";
                else
                    return "/Imagenes/Delete.png";
            }
        }
    }
}
