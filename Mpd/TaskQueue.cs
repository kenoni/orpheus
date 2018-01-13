/*This file is part of Orpheus.

   Orpheus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Orpheus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Orpheus.  If not, see<http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orpheus.Mpd
{
    public class TaskQueue
    {
        public SemaphoreSlim semaphoreSlim;
        public TaskQueue()
        {
            semaphoreSlim = new SemaphoreSlim(1,1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await semaphoreSlim.WaitAsync();
            //semaphore.WaitOne();
            try
            {
                await taskGenerator();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
