import { ThemeProvider } from "@emotion/react";
import PermissionForm from "./components/PermissionForm";
import { envs } from "./config"
import theme from "./theme";

function App() {

  const name = envs.API_URL;

  console.log(name);

  return (
    <ThemeProvider theme={theme}>
      <div className="App">
        <h1>Formulario de Permisos</h1>
        <PermissionForm />
      </div>
    </ThemeProvider>
  )
}

export default App
