const saveButton = document.querySelector('button');
const imagesContainer = document.querySelector('#image_gallery');
const imageInput = document.querySelector('#imagefile');
const imageName = document.querySelector('#name');
const imageDescription = document.querySelector('#description');
const imageLink = document.querySelector('#link');

saveButton.addEventListener('click', saveImage);

async function dispayImages(images)
{
    let allImages = '';

    images.forEach(image =>{
        const imageHtmlElement = `
        <div class="image" data-id="${image.id}">
            <img src="${image.imageLink}" alt="${image.name}">
            <h3>${image.name}</h3>
            <p class="description">${image.description}</p>
            <p class="time_created">${image.timeCreated}</p>
        </div>
        `;

        allImages += imageHtmlElement;
    })
    imagesContainer.innerHTML = allImages;
    
}

async function getAllImages()
{
    await fetch('https://localhost:7205/api/Images')
    .then(data => data.json())
    .then(response => dispayImages(response))
}

async function saveImage()
{
    if( (imageLink.value || imageInput.value) && !(imageLink.value && imageInput.value))
    {
        if(imageLink.value)
        {
            saveInfoAboutImage(imageLink.value);
        }
        else{
            const file = imageInput.files[0];
            const formData = new FormData();
            formData.append('file', file);

            await fetch('https://localhost:7205/api/Images/Upload',{
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => saveInfoAboutImage(data))
        }
    }
    else
    {
        alert('No image link was attached or no image file was attached');
    }
}

async function saveInfoAboutImage(path)
{
    const ImageData = {
        name: imageName.value,
        description: imageDescription.value,
        imageLink: path
    };
    await fetch('https://localhost:7205/api/Images', 
    {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(ImageData)
    })
    .then(response => response.json())
    .then(data => {
        getAllImages();
    })
    .catch(error => console.error(error));
}

getAllImages();