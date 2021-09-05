using System;

using Raven.Client.Documents;

namespace Server.Database {
	public class DocumentStoreHolder {
		static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

		public static IDocumentStore Store => store.Value;

		static IDocumentStore CreateStore() {
			var document_store = new DocumentStore() {
				Urls = new[] {
					"http://127.0.0.1:8080",
				},
				Database = "defsite",
			}.Initialize();

			return document_store;
		}
	}
}