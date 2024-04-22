using MsgPack.Serialization;
using VsMaker2Core.DataModels;

namespace VsMaker2Core.Methods
{
    public class FileSystemMethods : IFileSystemMethods
    {
        public (bool Success, string ErrorMessage) ExportTrainers(List<Trainer> trainers, string filePath)
        {
            var exportModel = new Trainers(trainers);
            try
            {
                var stream = new MemoryStream();
                var serializer = MessagePackSerializer.Get<Trainers>();
                serializer.Pack(stream, exportModel);
                stream.Position = 0;

                var fileSteam = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                stream.WriteTo(fileSteam);
                stream.Close();
                fileSteam.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        public (List<Trainer> Trainers, bool Success, string ErrorMessage) ImportTrainers(string filePath)
        {
            List<Trainer> trainers = [];
            try
            {
               // var serializer = MessagePackSerializer.;
            }
            catch (Exception ex)
            {
                return (trainers, false, ex.Message);
            }
            return (trainers, true, string.Empty);
        }
    }
}