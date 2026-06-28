import { Dialog, Transition } from "@headlessui/react";
import { Fragment } from "react/jsx-runtime";
import type { DataTown } from "../types/dataTypes";
import { Link } from "react-router-dom";

interface TownModalProps {
    isOpen: boolean
    closeModal: () => void
    town: DataTown | null
}


export default function TownModal({closeModal, isOpen, town} : TownModalProps) {
  return (
    <Transition appear show={isOpen} as={Fragment}>
        <Dialog as='div' className='relative z-50' onClose={closeModal}>
            <Transition.Child
                as={Fragment}
                enter="ease-out duration-300"
                enterFrom="opacity-0"
                enterTo="opacity-100"
                leave="ease-in duration-200"
                leaveFrom="opacity-100"
                leaveTo="opacity-0"
            >
                <div className="fixed inset-0 bg-black/60 dark:bg-black/60 aria-hidden:true"/>
            </Transition.Child>

            <div className="fixed inset-0 overflow-y-auto">
                <div className="flex min-h-full items-center justify-center p-4 text-center">
                    <Transition.Child
                        as={Fragment}
                        enter="ease-out duration-300"
                        enterFrom="opacity-0 scale-95"
                        enterTo="opacity-100 scale-100"
                        leave="ease-in duration-200"
                        leaveFrom="opacity-100 scale-100"
                        leaveTo="opacity-0 scale-95"
                    >
                        <Dialog.Panel className='w-full max-w-md rounded-2xl p-6 shadow-xl transition-all
                                     bg-white dark:bg-slate-900 border border-gray-100 dark:border-slate-800'>

                            <Dialog.Title as="h3" className='text-lg font-bold text-gray-900 dark:text-white mb-3'>
                             {town?.name}
                              
                            </Dialog.Title>
                        <img
                            src={town?.image}
                            alt={town?.name}
                            className="w-full object-cover  mb-4"
                        />

                        <div className="text-sm text-gray-900 dark:text-white mb-3">
                            {town?.description}
                        </div>

                        <div className="mt-2 flex justify-center">
                          <Link
                            to={`/towndetails/${town?.id}`}
                            className="inline-flex rounded-md border border-transparent bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800 focus:outline-none"
                          >Ver más</Link>

                        </div>
                      
                        </Dialog.Panel>
                    </Transition.Child>
                </div>
            </div>
        </Dialog>
    </Transition>
  )
}
