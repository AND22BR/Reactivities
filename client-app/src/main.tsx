import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'semantic-ui-css/semantic.min.css'
import './app/layouts/styles.css'
import App from './app/layouts/App.tsx'
import 'react-toastify/dist/ReactToastify.css'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { RouterProvider } from 'react-router'
import { router } from './app/router/Routes.tsx'
import { store, StoreContext } from './lib/stores/store.ts'
import { ToastContainer } from 'react-toastify'
import { LocalizationProvider } from '@mui/x-date-pickers'
import {AdapterDateFns} from '@mui/x-date-pickers/AdapterDateFns'

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <StoreContext.Provider value={store}>
      <QueryClientProvider client={queryClient}>
        <ReactQueryDevtools initialIsOpen={false} />
        <ToastContainer position='top-center' hideProgressBar theme='colored'/>
        <RouterProvider router={router}/>
      </QueryClientProvider>
    </StoreContext.Provider>
    </LocalizationProvider>
  </StrictMode>
)
