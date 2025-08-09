import { CloudUpload } from "@mui/icons-material";
import { Box, Button, Grid, Typography } from "@mui/material";
import React, {useCallback, useEffect, useRef, useState} from 'react'
import {useDropzone} from 'react-dropzone'
import Cropper, { ReactCropperElement } from "react-cropper";
import "cropperjs/dist/cropper.css";
import { file } from "zod";

type Props={
    uploadPhoto: (file: Blob) => void
    loading: boolean
}

export default function PhotoUploadWidget({uploadPhoto, loading}: Props) {
    const [files, setFiles]=useState<object & { preview: string; }[]>([]);
    const cropperRef=useRef<ReactCropperElement>(null);

    useEffect(()=>{
        return ()=> {
            files.forEach(file => URL.revokeObjectURL(file.preview))
        }
    }, [files]);
    
    const onDrop = useCallback((acceptedFiles: File[]) => {
    setFiles(acceptedFiles.map(file => Object.assign(file, {
        preview: URL.createObjectURL(file as Blob)
    })))
    // Do something with the files
  }, [])

  const onCrop=useCallback(()=>{
    const cropper=cropperRef.current?.cropper;
    cropper?.getCroppedCanvas().toBlob(blob => {
        uploadPhoto(blob as Blob)
    })
  },[])

  const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop})

  return (
    <Grid container spacing={3}>
        <Grid size={4}>
            <Typography variant="overline" color="secondary">Step 1 - Add photo</Typography>
            <Box {...getRootProps()}
                sx={{border: 'dashed 3px #eee',
                    borderColor: isDragActive? 'green' : '#eee',
                    borderRadius: '5px',
                    paddingTop: '30px',
                    textAlign: 'center',
                    height: '280px'
                }}>
            <input {...getInputProps()} />
            <CloudUpload sx={{fontSize: 80}}/>
            <Typography variant="h5">Drop image here</Typography>
            {
                isDragActive ?
                <p>Drop the files here ...</p> :
                <p>Drag 'n' drop some files here, or click to select files</p>
            }
            </Box>
        </Grid>
        <Grid size={4}>
            <Typography variant="overline" color="secondary">Step 2 - Resize image</Typography>
            {files[0]?.preview && 
            <Cropper ref={cropperRef}
                src={files[0]?.preview}
                style={{height: 300, width: '90%'}}
                initialAspectRatio={1}
                aspectRatio={1}
                preview='.img-preview'
                guides={false}
                viewMode={1}
                background={false}
                />
            }
            
        </Grid>
        <Grid size={4}>
            {files[0]?.preview && (
                <>
                    <Typography variant="overline" color="secondary">Step 3 -Preview Image</Typography>
                    <div className="img-preview"
                        style={{width: 300, height: 300, overflow: 'hidden'}}>
                    </div>
                    <Button 
                        sx={{my: 1, width: 300}}
                        onClick={onCrop}
                        variant="contained"
                        color="secondary"
                        disabled={loading}>Upload</Button>
                </>
            )}
        </Grid>
    </Grid>
  )
}