using OpenAI.Chat;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLMSandbox
{
    public class Embeddings
    {
        private readonly EmbeddingClient _client;
        public Embeddings(string apiKey)
        {
            _client = new EmbeddingClient(apiKey: apiKey, model: "text-embedding-3-small");
        }

        public async Task CreateMultipleEmbeddings()
        {
            string category = "Fiction";
            string description = "Many types of stories that are not based in real life. There are several great genres"
                + " and all are quite entertaining. The variety is fantastic -- drama, true crime, fantasy, sci-fi, all with many options"
                + " and authors. Fiction stories are present in many forms.";

            List<string> inputs = [category, description];

            OpenAIEmbeddingCollection collection = await _client.GenerateEmbeddingsAsync(inputs);

            foreach (OpenAIEmbedding embedding in collection)
            {
                ReadOnlyMemory<float> vector = embedding.ToFloats();

                Console.WriteLine($"Dimension: {vector.Length}");
                Console.WriteLine($"Floats: ");
                for (int i = 0; i < vector.Length; i++)
                {
                    Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
                }

                Console.WriteLine();
            }


        }
    }
}
