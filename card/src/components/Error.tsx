import type React from "react";



export default function Error({children} : {children: React.ReactNode}) {
  return (
    <div className=" text-red-500 font-bold text-sm text-center bg-gray-200 p-3 uppercase rounded-2xl shadow-2xl  mb-3 ">
        {children}
    </div>
  )
}
