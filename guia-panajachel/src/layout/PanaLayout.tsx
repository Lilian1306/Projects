import { Outlet } from "react-router-dom";
import DarkMode from "../utils/DarkMode";

export default function PanaLayout() {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 transition-colors duration-500">
      <header className="flex flex-row justify-between items-center w-full px-4 md:px-10 bg-sky-500 dark:bg-sky-900 shadow-md relative z-10 py-4 min-h-[100px] transition-colors duration-500">
  
        <div className="flex items-center gap-4">
          <img
            src="/logo.avif"
            alt="Logo Atitlan"
            className="h-20 md:h-24 w-auto rounded-xl shadow-2xl"
          />
          <h1 className="text-2xl md:text-4xl font-bold text-white tracking-widest drop-shadow-md">
            Panajachel
          </h1>
        </div>

        <div className="flex flex-col items-end gap-2 text-white font-medium">
          <span className="text-sm md:text-base border-b border-sky-300 dark:border-sky-700 pb-1">
            Guatemala 
          </span>
          <div className="mt-1">
            <DarkMode />
          </div>
        </div>

      </header>
      <section className="p-4">
        <Outlet />
      </section>
    </div>
  );
}