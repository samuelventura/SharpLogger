using System;
using System.Drawing;
using System.Windows.Forms;
using SharpTabs;

namespace SharpLogger.Viewer
{
    public class LogFactory : ISessionFactory
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

        public ISessionDto[] Load()
        {
            throw new NotImplementedException();
        }

        public ISessionDto[] Load(string path)
        {
            throw new NotImplementedException();
        }

        public Control New()
        {
            throw new NotImplementedException();
        }

        public void Save(ISessionDto[] dtos)
        {
            throw new NotImplementedException();
        }

        public void Save(string path, ISessionDto[] dtos)
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

        public ISessionDto Unwrap(Control control)
        {
            throw new NotImplementedException();
        }

        public Control Wrap(ISessionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
