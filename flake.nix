{
  inputs.nixpkgs.url = "nixpkgs/nixpkgs-unstable";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs =
    { nixpkgs, flake-utils, ... }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs {
          inherit system;
          config.allowUnfree = true;
        };
      in
      {
        devShells.default = pkgs.mkShell {
          DOTNET_WATCH_RESTART_ON_RUDE_EDIT = "1";
          DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER = "1";
          DOTNET_ENVIRONMENT = "Development";
          buildInputs = with pkgs; [
            dotnet-sdk_10
          ];
        };
      }
    );
}
