namespace GGPhys.Rigid
{
    public class Singleton<T> where T : class, new()
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }

                return m_Instance;
            }
        }

        public static void Destroy()
        {
            m_Instance = null;
        }
    }
}

