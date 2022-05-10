using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Shoping.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        //obtenidas las llave de azure necesitamos un objeto azure cliente para interacturar
        //se instala el apquete windowsazure.storage
        private readonly CloudBlobClient _blobClient;

        //necesitamos inyectar la opcion BLOB del appsetting
        //para eso se usa el IConfiguration que se inyecta en el constructor de la clase
        //para el caso solo necesitamo el iconfigration en el constructor pr eso no se crea su atributo
        public BlobHelper(IConfiguration configuration)
        {
            //almacenamos la llave del azure
            string keys = configuration["Blob:ConnectionString"];
            //creando la conexion a la cuenta de storage account de azure
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(keys);
            //accediendo al storage account en memoria, para su uso
            _blobClient = cloudStorageAccount.CreateCloudBlobClient();

        }

        public async Task DeleteBlobAsync(Guid id, string containerName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{id}");
            await blockBlob.DeleteAsync();
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            //caso cuando nos pasan el archivo
            //subiendo imagen al blob container
            //convirtiendo a stream
            Stream stream = file.OpenReadStream();

            return await UploadBlobAsync(stream, containerName);

        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadBlobAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            //caso cuando nos pasan el nombre del archivo
            //subiendo imagen al blob container
            //convirtiendo a stream
            Stream stream = File.OpenRead(image);
            return await UploadBlobAsync(stream, containerName);
        }

        private async Task<Guid> UploadBlobAsync(Stream stream, string containerName)
        {
            //generando id aleatorio
            Guid name = Guid.NewGuid();
            //accediendo al container
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //creando el blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            //subiendo foto al blob
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }

    }
}
