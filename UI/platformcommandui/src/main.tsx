import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './layout/App.tsx'
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { CssBaseline } from '@mui/material';
import "react-toastify/ReactToastify.css"
import { ToastContainer } from 'react-toastify';


createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ToastContainer position='bottom-right' hideProgressBar theme='colored' />
    <CssBaseline />
    <App />
  </StrictMode>,
)
