import { BrowserRouter, Route, Routes } from "react-router-dom";
import MainPage from "./views/MainPage";
import PanaLayout from "./layout/PanaLayout";
import TownDetails from "./components/TownDetails";


export default function Router() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<PanaLayout/>}>
                    <Route path="/" element={<MainPage/>} index />
                </Route>
                    <Route path="/towndetails/:id" element={<TownDetails/>}/>
            </Routes>    
        </BrowserRouter>
    )
 
}