# Script para registrar o protocolo personalizado "provahidrica"
$protocol = "provahidrica"
$command = '"C:\Program Files\Conecsa Automação e Ti\Prova Hidrica - Conecsa\ProvaHidrica.bat" "%1"'
$registryPath = "HKLM:\SOFTWARE\Classes\$protocol"
$commandPath = "$registryPath\Shell\Open\command"

# Função para verificar se a chave de registro existe
function Test-RegistryKey {
    param (
        [string]$Path
    )
    try {
        $null = Get-Item -Path $Path -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

# Cria a chave de registro para o protocolo
if (-not (Test-RegistryKey -Path $registryPath)) {
    New-Item -Path $registryPath -Force
    Set-ItemProperty -Path $registryPath -Name "(default)" -Value "URL:$protocol Protocol"
    Set-ItemProperty -Path $registryPath -Name "URL Protocol" -Value ""
    Write-Output "Chave de registro para o protocolo criada com sucesso."
} else {
    Write-Output "Chave de registro para o protocolo já existe."
}

# Cria a chave de registro para o comando
if (-not (Test-RegistryKey -Path $commandPath)) {
    New-Item -Path $commandPath -Force
    Set-ItemProperty -Path $commandPath -Name "(default)" -Value $command
    Write-Output "Chave de registro para o comando criada com sucesso."
} else {
    Write-Output "Chave de registro para o comando já existe."
}

Write-Output "Protocolo $protocol registrado com sucesso."
