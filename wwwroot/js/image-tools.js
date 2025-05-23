const uploadContainer = document.getElementById('upload-container');
const imageUploadInput = document.getElementById('image-upload');

uploadContainer.addEventListener('click', () => {
    imageUploadInput.click();
});

document.getElementById('select-button').addEventListener('click', (e) => {
    e.stopPropagation();
    imageUploadInput.click();
});

// Handle drag-over effect
uploadContainer.addEventListener('dragover', (e) => {
    e.preventDefault();
    uploadContainer.classList.add('dragover');
});

uploadContainer.addEventListener('dragleave', (e) => {
    e.preventDefault();
    uploadContainer.classList.remove('dragover');
});

uploadContainer.addEventListener('drop', (e) => {
    e.preventDefault();
    uploadContainer.classList.remove('dragover');

    if (e.dataTransfer.files.length > 0) {
        imageUploadInput.files = e.dataTransfer.files;

        const file = e.dataTransfer.files[0];
        const reader = new FileReader();
        reader.onload = function (event) {
            document.getElementById('uploaded-image-preview').src = event.target.result;
            document.getElementById('upload-container').style.display = 'none';
            document.getElementById('uploaded-image-section').style.display = 'block';
            document.getElementById('tools-grid').style.display = 'block';
        };
        reader.readAsDataURL(file);
        
        uploadSelectedFile(file);
    }
});

// Still handle normal file input change
imageUploadInput.addEventListener('change', function (e) {
    if (e.target.files.length > 0) {
        const file = e.target.files[0];
        const reader = new FileReader();
        reader.onload = function (event) {
            document.getElementById('uploaded-image-preview').src = event.target.result;
            document.getElementById('upload-container').style.display = 'none';
            document.getElementById('uploaded-image-section').style.display = 'block';
            document.getElementById('tools-grid').style.display = 'block';

        };
        reader.readAsDataURL(file);
        
        uploadSelectedFile(file);
    }
});

document.getElementById('upload-another').addEventListener('click', function (e) {
    e.stopPropagation();
    imageUploadInput.value = ""; // Clear file input
    document.getElementById('upload-container').style.display = 'block';
    document.getElementById('uploaded-image-section').style.display = 'none';
    document.getElementById('tools-grid').style.display = 'none';
    document.getElementById('result-container').style.display = 'none';
});

document.querySelectorAll('.tool-card').forEach(card => {
    card.addEventListener('click', function () {
        const toolId = this.getAttribute('data-toolid');
        const fileId = document.getElementById("uploaded-file-id").value;
        // Disable buttons
        document.querySelectorAll('.tool-card').forEach(c => c.classList.add('disabled'));

        // Show loading spinner
        document.getElementById('result-container').style.display = 'block';
        document.getElementById('loading-indicator').style.display = 'block';
        document.getElementById('result-image').style.display = 'none';
        document.getElementById('download-result').style.display = 'none';

        const formData = new FormData();
        formData.append('file', fileId);
        formData.append('toolId', toolId); // <-- use toolId instead of toolName

        fetch('/Tools/ProcessImage', {
            method: 'POST',
            body: formData
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    document.getElementById('result-image').src = data.resultUrl;
                    document.getElementById('result-image').style.display = 'block';

                    document.getElementById('download-result').href = data.resultUrl;
                    document.getElementById('download-result').setAttribute('download', data.downloadName);
                    document.getElementById('download-result').style.display = 'inline-block';

                    document.getElementById('loading-indicator').style.display = 'none';
                } else {
                    showError('Processing failed. Please try again.');
                }

            }).catch(err => {
                console.error(err);
                showError('An unexpected error occurred while processing the image.');
            })
            .finally(() => {
                document.getElementById('loading-indicator').style.display = 'none';
                document.querySelectorAll('.tool-card').forEach(c => c.classList.remove('disabled'));
            });
    });
});

function uploadSelectedFile(file)
{
    const formData = new FormData();
    formData.append('file', file);
    
    fetch('/Tools/SaveUploadedFile', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(data =>
        {
            if(data.success)
            {
                const fileId = data.fileId;
                
                const url = new URL(window.location.href);
                url.searchParams.set("file", fileId);
                
                history.replaceState(null, "", url);
                
                document.getElementById('uploaded-file-id').value = fileId;
            }
        })
        .catch(err => console.log(err));
}

function showError(message) {
    let errorContainer = document.getElementById('error-message');
    if (!errorContainer) {
        errorContainer = document.createElement('div');
        errorContainer.id = 'error-message';
        errorContainer.className = 'alert alert-danger mt-3';
        document.getElementById('result-container').appendChild(errorContainer);
    }

    errorContainer.textContent = message;
    errorContainer.style.display = 'block';
}


