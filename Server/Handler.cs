using Defsite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace Server {
	public abstract class Handler {
		object update_clients_lock = new object();

		public List<Client> Clients = new List<Client>();

		long now = DateTime.Now.Ticks;
		long delta_time = 0;

		public virtual int TPS => 20;

		public virtual void UpdateClients(List<Client> client_list) {
			lock (update_clients_lock) {
				Clients = client_list;
			}
		}

		public virtual void Update() {

		}

		public virtual void FixedUpdate(long delta_time) {

		}

		public virtual void Run(bool fixed_time_step = false) {
			UpdateClients(Server.GetClients());

			if (fixed_time_step) {
				now = DateTime.Now.Millisecond;
				FixedUpdate(delta_time);
				delta_time = DateTime.Now.Millisecond - now;
				if (delta_time < (1000 / TPS)) Thread.Sleep((1000 / TPS) - (int)delta_time);
			} else {
				Update();
			}

		}
	}
}
