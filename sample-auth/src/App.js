import {
    BrowserRouter,
    Routes,
    Route
} from "react-router-dom";
import './App.css';
import CallbackComponent from './components/CallbackComponent';
import HelloWorldComponent from './components/HelloWorldComponent';
import NotFoundComponent from "./components/NotFoundComponent";
import { ProvideAuth } from './hooks/useAuth';
import './utils/axiosUtils'
import history from "./utils/history";

function App() {

    return <ProvideAuth>
        <BrowserRouter history={history}>
            <Routes>
                <Route exact={true} path="/" element={<HelloWorldComponent />}>
                </Route>
                <Route exact={true} path="/TestCallback" element={<HelloWorldComponent />}>
                </Route>
                <Route exact={true} path="callback" element={<CallbackComponent />}>
                </Route>

                <Route exact={true} path="*" element={<NotFoundComponent />} />
            </Routes>
        </BrowserRouter>
    </ProvideAuth>
}
export default App;
