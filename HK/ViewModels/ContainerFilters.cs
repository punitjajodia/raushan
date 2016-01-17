using HK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class ContainerFilters
    {
        public List<Exporter> Exporters;
        public List<Importer> Importers;
        public List<int> SelectedExporterIDs;
        public List<int> SelectedImporterIDs;

    }
}