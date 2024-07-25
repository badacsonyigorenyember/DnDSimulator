using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;

namespace Tests.EditMode {
    public class GameStateSerializationTest {
        private GameStateDto _gameStateDto;

        [SetUp]
        public void SetUp() {
            _gameStateDto = new GameStateDto();
            _gameStateDto.sceneData = "Ékezetes betűk a szerializáció miatt!";
            _gameStateDto.mapPicture = new byte[] { 10, 11, 12, 13 };
            _gameStateDto.creaturePictures = new Dictionary<string, byte[]>() {
                { "egy", new byte[] { 1, 2, 3, 4 } },
                { "ketto", new byte[] { 5, 4, 3, 2 } }
            };
        }

        [Test]
        public void TestGameStateJsonKuldes() {
            string gameStateJson = JsonConvert.SerializeObject(_gameStateDto);

            int bufferSize = FastBufferWriter.GetWriteSize(gameStateJson);
            using (FastBufferWriter fastBufferWriter = new FastBufferWriter(bufferSize, Allocator.Temp)) {
                if (!fastBufferWriter.TryBeginWrite(bufferSize)) {
                    Assert.Fail();
                }

                fastBufferWriter.WriteValueSafe(gameStateJson);

                using (FastBufferReader reader = new FastBufferReader(fastBufferWriter, Allocator.Temp)) {
                    if (!reader.TryBeginRead(bufferSize)) {
                        Assert.Fail();
                    }

                    reader.ReadValueSafe(out string readString);
                    Assert.That(gameStateJson == readString);
                }
            }

            Assert.Pass();
        }

        [Test]
        public void TestGameStateJsonSerialization() {
            string gameStateJson = JsonConvert.SerializeObject(_gameStateDto);
            GameStateDto gameStateRecreated = JsonConvert.DeserializeObject<GameStateDto>(gameStateJson);

            // Basic field-ek
            Assert.That(_gameStateDto.sceneData == gameStateRecreated.sceneData);
            Assert.That(_gameStateDto.mapPicture.SequenceEqual(gameStateRecreated.mapPicture));

            // Dictionary
            DictionaryEquals(_gameStateDto.creaturePictures, gameStateRecreated.creaturePictures);
        }

        private static void DictionaryEquals<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2) {
            Assert.That(dict1, Is.Not.Null);
            Assert.That(dict2, Is.Not.Null);
            Assert.That(dict1.Count, Is.EqualTo(dict2.Count));

            foreach (var kvp in dict1) {
                Assert.That(dict2.ContainsKey(kvp.Key), $"Key '{kvp.Key}' is missing in the second dictionary.");
                Assert.That(dict2[kvp.Key], Is.EqualTo(kvp.Value), $"Value for key '{kvp.Key}' does not match.");
            }
        }
    }
}
