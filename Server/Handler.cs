using System;
using System.Collections.Generic;
using System.Threading;
using Defsite;

namespace Server {
	public abstract class Handler {
		protected List<Client> Clients = new List<Client>();
		long delta_time = 0;

		long now = DateTime.Now.Ticks;
		object update_clients_lock = new object();

		protected virtual int TPS => 20;

		void UpdateClients(List<Client> client_list) {
			lock (update_clients_lock) {
				Clients = client_list;
			}
		}

		protected virtual void Update() {
		}

		protected virtual void FixedUpdate(long delta_time) {
		}

		public void Run(bool fixed_time_step = false) {
			UpdateClients(Server.GetClients());

			if (fixed_time_step) {
				now = DateTime.Now.Millisecond;
				FixedUpdate(delta_time);
				delta_time = DateTime.Now.Millisecond - now;
				if (delta_time < 1000 / TPS) Thread.Sleep(1000 / TPS - (int) delta_time);
			}
			else {
				Update();
			}
		}
	}
}