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
    let button;

    images.forEach(image =>{
        const imageHtmlElement = `
        <div class="image-container">
            <div class="image-wrapper">
                <img src="${image.imageLink}" alt="${image.name}">
                <h3>${image.name}</h3>
            </div>
            <div class="image-bottom">
                <p class="description">${image.description}</p>
                <p class="time_created">${image.timeCreated}</p>
            </div>
        </div>
        `;

        // <div class="image-bottom-right">
        // <img class="delete" src="Images/delete.png" alt="delete" id="delete" data-id="${image.id}">
        // </div>
        // button = document.querySelector('#delete');
        // button.addEventListener('click', () => {
        //     const id = button.dataset.id;
        //     deleteImage(id);
        // });
        // deleteButtons[image.id] = document.querySelector(`#delete${image.id}`)
        // deleteButtons[image.id].addEventListener('click', deleteImage(image.id));
        allImages += imageHtmlElement;
    })
    imagesContainer.innerHTML = allImages;

    const deleteButtons = document.querySelectorAll('#delete');

    deleteButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const id = button.dataset.id;

            console.log(`Delete button clicked for item ${id}`);
            deleteImage(id);
        });
    });
    
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
    const response = await fetch('https://localhost:7205/api/Images', 
    {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(ImageData)
    })
    if(response.ok === true)
    {
        await getAllImages();
    }
}

async function deleteImage(id)
{
    const response = await fetch(`https://localhost:7205/api/Images/${id}`, 
    {
        method: 'DELETE',
    })
    if(response.ok === true)
    {
        await getAllImages();
    }
}

getAllImages();