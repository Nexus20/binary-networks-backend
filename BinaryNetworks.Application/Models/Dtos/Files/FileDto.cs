﻿using BinaryNetworks.Application.Extensions;

namespace BinaryNetworks.Application.Models.Dtos.Files;

public class FileDto
{
    public Stream Content { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    
    public string GetPathWithFileName()
    {
        var uniqueRandomFileName = Path.GetRandomFileName();
        var shortClientSideFileNameWithoutExtension = Path.GetFileNameWithoutExtension(Name).TruncateLongString(30);
        var fileExtension = Path.GetExtension(Name);

        return uniqueRandomFileName + "_" + shortClientSideFileNameWithoutExtension + fileExtension;
    }
}