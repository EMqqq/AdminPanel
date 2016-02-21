using AdminPanel.Abstract;
using AdminPanel.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AdminPanel.DataAccessLayer
{
    public class FilePathRepository : TRepository<AdminPanelContext, FilePath>
    {
        public FilePathRepository(AdminPanelContext context) : base(context) { }

        public override void Delete(FilePath filePath)
        {
            Entities.Remove(filePath);
        }
    }
}