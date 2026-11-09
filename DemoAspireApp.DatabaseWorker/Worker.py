import os
import asyncio
import aiohttp
from datetime import datetime

class Worker:
    async def check_tasks(self) -> None:
        tasks_api = os.getenv('services__apiservice__https__0')
        print(f"Using API: {tasks_api}", flush=True)
        
        if not tasks_api:
            print("Error: TASKS_API base URL not configured", flush=True)
            return
        
        try:
            url = f"{tasks_api}/tasks"
            async with aiohttp.ClientSession() as session:
                async with session.get(url) as response:
                    response.raise_for_status()
                    tasks = await response.json()
            print(f"Retrieved {len(tasks)} tasks", flush=True)
            
            overdue_tasks = self.get_overdue_tasks(tasks)
            if overdue_tasks:
                print(f"\nOverdue tasks ({len(overdue_tasks)}):", flush=True)
                for task in overdue_tasks:
                    print(f"  - {task['description']}", flush=True)
            else:
                print("\nNo overdue tasks found", flush=True)
        except (aiohttp.ClientError, asyncio.TimeoutError) as e:
            print(f"Error fetching tasks from API: {e}", flush=True)
    
    def get_overdue_tasks(self, tasks: list) -> list:
        overdue = []
        now = datetime.now()
        
        for task in tasks:
            try:
                deadline = datetime.fromisoformat(task['deadline'].replace('Z', '+00:00'))
                if deadline < now:
                    overdue.append(task)
            except (ValueError, KeyError) as e:
                print(f"Error processing task {task.get('id', 'unknown')}: {e}", flush=True)
        
        return overdue

async def main() -> None:
    worker = Worker()
    while True:
        print("Checking tasks...", flush=True)
        await worker.check_tasks()
        print("Sleeping for 5 seconds...", flush=True)
        await asyncio.sleep(5)

if __name__ == "__main__":
    asyncio.run(main())