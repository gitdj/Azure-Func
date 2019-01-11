using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace AzureFunction.Helper
{
    public class TransformHelper
    {

        public string Transform(string mapType, string req)
        {
            //return XslCompiledTransform();
            string input = req;
            Stream stylesheet = null;
            Stream extension_object = null;

            BizTalkMapType MapType = BizTalkMapType.InToOutMap;
            
            MapInitializer(ref input, ref stylesheet, ref extension_object, MapType);


           // byte[] mapContent = await blobStorageClient.GetBlob(_mapsContainerName, transformRequest.MapName);
            //byte[] extensionObjectContent = transformRequest.ExtensionObjectName == null ? null : await blobStorageClient.GetBlob(_extensionObjectsContainerName, transformRequest.ExtensionObjectName);


            return XslCompiledTransform(input, stylesheet, extension_object);
        }
        


        public async Task<string> ExecuteTransform(byte[] inputXml, string mapName)
        {
            Stream stylesheet = null;
            Stream extension_object = null;


            BizTalkMapType mapType = (BizTalkMapType)Enum.Parse(typeof(BizTalkMapType), mapName);

            Initialize(ref stylesheet, ref extension_object, mapType);

            object[] extensionObjects = await GetExtensionObjects(extension_object);

            return await XslCompiledTransforms(
                XmlReader.Create(new MemoryStream(inputXml)),
                stylesheet,extensionObjects);
        }


        //private async Task<byte[]> XslCompiledTransform(string document, Stream stylesheet, Stream extensions)
        //{
        //    return XslCompiledTransforms(document, stylesheet, ParseExtension(extensions));
        //}


        private static string XslCompiledTransform(string document, Stream stylesheet, Stream extensions)
        {
            return XslCompiledTransforms(document, stylesheet, ParseExtension(extensions));
        }


        private async Task<string> XslCompiledTransforms(XmlReader inputDocument, Stream stylesheet, object[] extension)
        {
            var resolver = new XmlUrlResolver();
            XslCompiledTransform transform = new XslCompiledTransform(true);
            transform.Load(XmlReader.Create(stylesheet), new XsltSettings(true, true), resolver);
            XsltArgumentList arguments = new XsltArgumentList();
            for (int index = 0; index < extension.Length; index += 2)
            {
                arguments.AddExtensionObject(
                extension[index] as string,
                extension[index + 1]);
            }

            var outputStream = new MemoryStream();
            
            using (XmlWriter writer = XmlWriter.Create(outputStream, transform.OutputSettings))
                transform.Transform(inputDocument, arguments, writer);
            outputStream.Position = 0;
            StreamReader reader = new StreamReader(outputStream);
            return reader.ReadToEnd();

            //return Convert.ToBase64String(outputStream.ToArray());
        }

        private static string XslCompiledTransforms(string document, Stream stylesheet, object[] extension)
        {
            var resolver = new XmlUrlResolver();
            XslCompiledTransform transform = new XslCompiledTransform(true);
            transform.Load(XmlReader.Create(stylesheet), new XsltSettings(true, true), resolver);
            XsltArgumentList arguments = new XsltArgumentList();
            for (int index = 0; index < extension.Length; index += 2)
            {
                arguments.AddExtensionObject(
                extension[index] as string,
                extension[index + 1]);
            }
            StringBuilder output = new StringBuilder();
            var stream = new MemoryStream();
            var inputwriter = new StreamWriter(stream);
            inputwriter.Write(document);
            inputwriter.Flush();
            stream.Position = 0;
            
            using (XmlWriter writer = XmlWriter.Create(output, transform.OutputSettings))
                transform.Transform(XmlReader.Create(stream), arguments, writer);
            return output.ToString();
        }

        private async Task<object[]> GetExtensionObjects(Stream extensionObjectXml)
        {
            if ((extensionObjectXml.Length == 0))
                return new object[] { };
            XmlDocument document = new XmlDocument();
            document.Load(extensionObjectXml);
            ArrayList extensions = new ArrayList();
            foreach (XmlNode node in document.SelectNodes("/ExtensionObjects/ExtensionObject"))
            {
                string extension_namespace = node.Attributes["Namespace"].Value;
                string extension_assembly = node.Attributes["AssemblyName"].Value;
                string extension_class = node.Attributes["ClassName"].Value;
                string assembly_qualified_name = String.Format("{0}, {1}"
                , extension_class
                , extension_assembly
                ); object extension_object = Activator.CreateInstance(Type.GetType(assembly_qualified_name));
                extensions.Add(extension_namespace);
                extensions.Add(extension_object);
            }

            return extensions.ToArray();
        }
        private static object[] ParseExtension(Stream extension)
        {
            if ((extension.Length==0))
                return new object[] { };
            XmlDocument document = new XmlDocument();
            document.Load(extension);
            ArrayList extensions = new ArrayList();
            foreach (XmlNode node in document.SelectNodes("/ExtensionObjects/ExtensionObject"))
            {
                string extension_namespace = node.Attributes["Namespace"].Value;
                string extension_assembly = node.Attributes["AssemblyName"].Value;
                string extension_class = node.Attributes["ClassName"].Value;
                string assembly_qualified_name = String.Format("{0}, {1}"
                , extension_class
                , extension_assembly
                ); object extension_object = Activator.CreateInstance(Type.GetType(assembly_qualified_name));
                extensions.Add(extension_namespace);
                extensions.Add(extension_object);
            }

            return extensions.ToArray();
        }

        private void Initialize(ref Stream stylesheet, ref Stream extension_object, BizTalkMapType mapType)
        {   
            switch (mapType)
            {
                case BizTalkMapType.InToOutMap:
                    //input =  @"\XSLT\IOFiles\MsgIn_output.xml";
                    stylesheet = GetFromResources("XSLT.InToOut.xsl");
                    extension_object = GetFromResources("XSLT.InToOut_extxml.xml");
                    break;
            }
        }

        private static void MapInitializer(ref string input, ref Stream stylesheet, ref Stream extension_object, BizTalkMapType MapType)
        {
            string basedirectorypath = "";
            switch (MapType)
            {
                case BizTalkMapType.InToOutMap:
                    //input =  @"\XSLT\IOFiles\MsgIn_output.xml";
                    stylesheet = GetFromResources("XSLT.InToOut.xsl");
                    extension_object = GetFromResources("XSLT.InToOut_extxml.xml");
                    break;
            }
        }
        private enum BizTalkMapType
        {
            InToOutMap = 2,
            OneToOneMap = 1

        }

        internal static Stream GetFromResources(string resourceName)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            return assem.GetManifestResourceStream(assem.GetName().Name + '.' + resourceName);
        }
    }
}

