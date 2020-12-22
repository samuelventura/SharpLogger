using System;
using System.Drawing;
using System.Windows.Forms;
using SharpTabs;

namespace SharpLogger.Viewer
{
    public class LogFactory : SessionFactory
    {
        public LogFactory(string pth)
        {
        }

        public bool HasSetup => throw new NotImplementedException();

        public Icon Icon => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string Ext => throw new NotImplementedException();

        public string Title => throw new NotImplementedException();

        public string Status => throw new NotImplementedException();

        public SessionDto[] Load()
        {
            throw new NotImplementedException();
        }

        public SessionDto[] Load(string path)
        {
            throw new NotImplementedException();
        }

        public Control New()
        {
            throw new NotImplementedException();
        }

        public void Save(SessionDto[] dtos)
        {
            throw new NotImplementedException();
        }

        public void Save(string path, SessionDto[] dtos)
        {
            throw new NotImplementedException();
        }

        public void Setup(Control control)
        {
            throw new NotImplementedException();
        }

        public void Unload(Control control)
        {
            throw new NotImplementedException();
        }

        public SessionDto Unwrap(Control control)
        {
            throw new NotImplementedException();
        }

        public Control Wrap(SessionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
